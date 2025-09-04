// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Xml;

namespace MyNet.Xaml.Html;

/// <summary>
/// HtmlToXamlConverter is a static class that takes an HTML string
/// and converts it into XAML
/// </summary>
public static class HtmlToXamlConverter
{
    // ---------------------------------------------------------------------
    //
    // Internal Methods
    //
    // ---------------------------------------------------------------------

    #region Internal Methods

    /// <summary>
    /// Converts an html string into xaml string.
    /// </summary>
    /// <param name="htmlString">
    /// Input html which may be badly formated xml.
    /// </param>
    /// <param name="asFlowDocument">
    /// true indicates that we need a FlowDocument as a root element;
    /// false means that Section or Span elements will be used
    /// dependeing on StartFragment/EndFragment comments locations.
    /// </param>
    /// <returns>
    /// Well-formed xml representing XAML equivalent for the input html string.
    /// </returns>
    public static string ConvertHtmlToXaml(string htmlString, bool asFlowDocument)
    {
        // Create well-formed Xml from Html string
        var htmlElement = HtmlParser.ParseHtml(htmlString);

        // Decide what name to use as a root
        var rootElementName = asFlowDocument ? XamlFlowDocument : XamlSection;

        // Create an XmlDocument for generated xaml
        var xamlTree = new XmlDocument();
        var xamlFlowDocumentElement = xamlTree.CreateElement(null, rootElementName, XamlNamespace);

        // Extract style definitions from all STYLE elements in the document
        var stylesheet = new CssStylesheet(htmlElement);

        // Source context is a stack of all elements - ancestors of a parentElement
        var sourceContext = new List<XmlElement>(10);

        // Clear fragment parent
        _inlineFragmentParentElement = null;

        // convert root html element
        _ = AddBlock(xamlFlowDocumentElement, htmlElement, [], stylesheet, sourceContext);

        // In case if the selected fragment is inline, extract it into a separate Span wrapper
        if (!asFlowDocument)
        {
            xamlFlowDocumentElement = ExtractInlineFragment(xamlFlowDocumentElement);
        }

        // Return a string representing resulting Xaml
        xamlFlowDocumentElement.SetAttribute("xml:space", "preserve");
        var xaml = xamlFlowDocumentElement.OuterXml;

        return xaml;
    }

    /// <summary>
    /// Returns a value for an attribute by its name (ignoring casing)
    /// </summary>
    /// <param name="element">
    /// XmlElement in which we are trying to find the specified attribute
    /// </param>
    /// <param name="attributeName">
    /// String representing the attribute name to be searched for
    /// </param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "Impossible on XmlAttributeCollection")]
    internal static string? GetAttribute(XmlElement element, string attributeName)
    {
        if (!string.IsNullOrEmpty(attributeName) && element != null)
        {
            attributeName = attributeName.ToLower(CultureInfo.InvariantCulture);

            foreach (XmlAttribute item in element.Attributes)
                if (item.Name.Equals(attributeName, StringComparison.CurrentCultureIgnoreCase))
                    return item.Value;
        }

        return null;
    }

    /// <summary>
    /// Returns string extracted from quotation marks
    /// </summary>
    /// <param name="value">
    /// String representing value enclosed in quotation marks
    /// </param>
    internal static string UnQuote(string value)
    {
        if (value.StartsWith("\"", StringComparison.OrdinalIgnoreCase) && value.EndsWith("\"", StringComparison.OrdinalIgnoreCase) || value.StartsWith("'", StringComparison.OrdinalIgnoreCase) && value.EndsWith("'", StringComparison.OrdinalIgnoreCase))
        {
            value = value[1..^1].Trim();
        }
        return value;
    }

    #endregion Internal Methods

    // ---------------------------------------------------------------------
    //
    // Private Methods
    //
    // ---------------------------------------------------------------------

    #region Private Methods

    /// <summary>
    /// Analyzes the given htmlElement expecting it to be converted
    /// into some of xaml Block elements and adds the converted block
    /// to the children collection of xamlParentElement.
    /// 
    /// Analyzes the given XmlElement htmlElement, recognizes it as some HTML element
    /// and adds it as a child to a xamlParentElement.
    /// In some cases several following siblings of the given htmlElement
    /// will be consumed too (e.g. LIs encountered without wrapping UL/OL, 
    /// which must be collected together and wrapped into one implicit List element).
    /// </summary>
    /// <param name="xamlParentElement">
    /// Parent xaml element, to which new converted element will be added
    /// </param>
    /// <param name="htmlNode">
    /// Source html element subject to convert to xaml.
    /// </param>
    /// <param name="inheritedProperties">
    /// Properties inherited from an outer context.
    /// </param>
    /// <param name="stylesheet"></param>
    /// <param name="sourceContext"></param>
    /// <returns>
    /// Last processed html node. Normally it should be the same htmlElement
    /// as was passed as a paramater, but in some irregular cases
    /// it could one of its following siblings.
    /// The caller must use this node to get to next sibling from it.
    /// </returns>
    private static XmlNode? AddBlock(XmlElement xamlParentElement, XmlNode? htmlNode, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        if (htmlNode is XmlComment comment)
        {
            DefineInlineFragmentParent(comment, null);
        }
        else if (htmlNode is XmlText)
        {
            htmlNode = AddImplicitParagraph(xamlParentElement, htmlNode, inheritedProperties, stylesheet, sourceContext);
        }
        else if (htmlNode is XmlElement htmlElement) // Identify element name
        {
            var htmlElementName = htmlElement.LocalName; // Keep the name case-sensitive to check xml names
            var htmlElementNamespace = htmlElement.NamespaceURI;

            if (htmlElementNamespace != HtmlParser.XhtmlNamespace)
            {
                // Non-html element. skip it
                // Isn't it too agressive? What if this is just an error in html tag name?
                return htmlElement;
            }

            // Put source element to the stack
            sourceContext.Add(htmlElement);

            // Convert the name to lowercase, because html elements are case-insensitive
            htmlElementName = htmlElementName.ToLower(CultureInfo.InvariantCulture);

            // Switch to an appropriate kind of processing depending on html element name
            switch (htmlElementName)
            {
                // Sections:
                case "html":
                case "body":
                case "div":
                case "form": // not a block according to xhtml spec
                case "pre": // Renders text in a fixed-width font
                case "blockquote":
                case "caption":
                case "center":
                case "cite":
                    AddSection(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                    break;

                // Paragraphs:
                case "p":
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                case "nsrtitle":
                case "textarea":
                case "dd": // ???
                case "dl": // ???
                case "dt": // ???
                case "tt": // ???
                    AddParagraph(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                    break;

                case "ol":
                case "ul":
                case "dir": //  treat as UL element
                case "menu": //  treat as UL element
                    // List element conversion
                    AddList(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                    break;
                case "li":
                    // LI outside of OL/UL
                    // Collect all sibling LIs, wrap them into a List and then proceed with the element following the last of LIs
                    htmlNode = AddOrphanListItems(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                    break;

                case "table":
                    // hand off to table parsing function which will perform special table syntax checks
                    AddTable(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                    break;

                case "style": // We already pre-processed all style elements. Ignore it now
                case "meta":
                case "head":
                case "title":
                case "script":
                    // Ignore these elements
                    break;

                default:
                    // Wrap a sequence of inlines into an implicit paragraph
                    htmlNode = AddImplicitParagraph(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                    break;
            }

            // Remove the element from the stack
            Debug.Assert(sourceContext.Count > 0 && sourceContext[^1] == htmlElement);
            sourceContext.RemoveAt(sourceContext.Count - 1);
        }

        // Return last processed node
        return htmlNode;
    }

    // .............................................................
    //
    // Line Breaks
    //
    // .............................................................

    private static void AddBreak(XmlElement xamlParentElement, string htmlElementName)
    {
        // Create new xaml element corresponding to this html element
        var xamlLineBreak = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/XamlLineBreak, XamlNamespace);
        _ = xamlParentElement.AppendChild(xamlLineBreak);
        if (htmlElementName == "hr")
        {
            var xamlHorizontalLine = xamlParentElement.OwnerDocument.CreateTextNode("----------------------");
            _ = xamlParentElement.AppendChild(xamlHorizontalLine);
            xamlLineBreak = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/XamlLineBreak, XamlNamespace);
            _ = xamlParentElement.AppendChild(xamlLineBreak);
        }
    }

    // .............................................................
    //
    // Text Flow Elements
    //
    // .............................................................

    /// <summary>
    /// Generates Section or Paragraph element from DIV depending whether it contains any block elements or not
    /// </summary>
    /// <param name="xamlParentElement">
    /// XmlElement representing Xaml parent to which the converted element should be added
    /// </param>
    /// <param name="htmlElement">
    /// XmlElement representing Html element to be converted
    /// </param>
    /// <param name="inheritedProperties">
    /// properties inherited from parent context
    /// </param>
    /// <param name="stylesheet"></param>
    /// <param name="sourceContext">
    /// true indicates that a content added by this call contains at least one block element
    /// </param>
    private static void AddSection(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        // Analyze the content of htmlElement to decide what xaml element to choose - Section or Paragraph.
        // If this Div has at least one block child then we need to use Section, otherwise use Paragraph
        var htmlElementContainsBlocks = false;
        for (var htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling)
        {
            if (htmlChildNode is XmlElement element)
            {
                var htmlChildName = element.LocalName.ToLower(CultureInfo.InvariantCulture);
                if (HtmlSchema.IsBlockElement(htmlChildName))
                {
                    htmlElementContainsBlocks = true;
                    break;
                }
            }
        }

        if (!htmlElementContainsBlocks)
        {
            // The Div does not contain any block elements, so we can treat it as a Paragraph
            AddParagraph(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
        }
        else
        {
            // The Div has some nested blocks, so we treat it as a Section

            // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
            var currentProperties = GetElementProperties(htmlElement, inheritedProperties, out var localProperties, stylesheet, sourceContext);

            // Create a XAML element corresponding to this html element
            var xamlElement = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/XamlSection, XamlNamespace);
            ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/true);

            // Decide whether we can unwrap this element as not having any formatting significance.
            if (!xamlElement.HasAttributes)
            {
                // This elements is a group of block elements whitout any additional formatting.
                // We can add blocks directly to xamlParentElement and avoid
                // creating unnecessary Sections nesting.
                xamlElement = xamlParentElement;
            }

            // Recurse into element subtree
            for (var htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode?.NextSibling)
            {
                htmlChildNode = AddBlock(xamlElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
            }

            // Add the new element to the parent.
            if (xamlElement != xamlParentElement)
            {
                _ = xamlParentElement.AppendChild(xamlElement);
            }
        }
    }

    /// <summary>
    /// Generates Paragraph element from P, H1-H7, Center etc.
    /// </summary>
    /// <param name="xamlParentElement">
    /// XmlElement representing Xaml parent to which the converted element should be added
    /// </param>
    /// <param name="htmlElement">
    /// XmlElement representing Html element to be converted
    /// </param>
    /// <param name="inheritedProperties">
    /// properties inherited from parent context
    /// </param>
    /// <param name="stylesheet"></param>
    /// <param name="sourceContext">
    /// true indicates that a content added by this call contains at least one block element
    /// </param>
    private static void AddParagraph(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
        var currentProperties = GetElementProperties(htmlElement, inheritedProperties, out var localProperties, stylesheet, sourceContext);

        // Create a XAML element corresponding to this html element
        var xamlElement = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/XamlParagraph, XamlNamespace);
        ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/true);

        // Recurse into element subtree
        for (var htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling)
        {
            AddInline(xamlElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
        }

        // Add the new element to the parent.
        _ = xamlParentElement.AppendChild(xamlElement);
    }

    /// <summary>
    /// Creates a Paragraph element and adds all nodes starting from htmlNode
    /// converted to appropriate Inlines.
    /// </summary>
    /// <param name="xamlParentElement">
    /// XmlElement representing Xaml parent to which the converted element should be added
    /// </param>
    /// <param name="htmlNode">
    /// XmlNode starting a collection of implicitly wrapped inlines.
    /// </param>
    /// <param name="inheritedProperties">
    /// properties inherited from parent context
    /// </param>
    /// <param name="stylesheet"></param>
    /// <param name="sourceContext">
    /// true indicates that a content added by this call contains at least one block element
    /// </param>
    /// <returns>
    /// The last htmlNode added to the implicit paragraph
    /// </returns>
    private static XmlNode? AddImplicitParagraph(XmlElement xamlParentElement, XmlNode? htmlNode, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        // Collect all non-block elements and wrap them into implicit Paragraph
        var xamlParagraph = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/XamlParagraph, XamlNamespace);
        XmlNode? lastNodeProcessed = null;
        while (htmlNode != null)
        {
            if (htmlNode is XmlComment comment)
            {
                DefineInlineFragmentParent(comment, /*xamlParentElement:*/null);
            }
            else if (htmlNode is XmlText)
            {
                if (htmlNode!.Value!.Trim().Length > 0)
                {
                    AddTextRun(xamlParagraph, htmlNode.Value);
                }
            }
            else if (htmlNode is XmlElement element)
            {
                var htmlChildName = element.LocalName.ToLower(CultureInfo.InvariantCulture);
                if (HtmlSchema.IsBlockElement(htmlChildName))
                {
                    // The sequence of non-blocked inlines ended. Stop implicit loop here.
                    break;
                }
                else
                {
                    AddInline(xamlParagraph, element, inheritedProperties, stylesheet, sourceContext);
                }
            }

            // Store last processed node to return it at the end
            lastNodeProcessed = htmlNode;
            htmlNode = htmlNode.NextSibling;
        }

        // Add the Paragraph to the parent
        // If only whitespaces and commens have been encountered,
        // then we have nothing to add in implicit paragraph; forget it.
        if (xamlParagraph.FirstChild != null)
        {
            _ = xamlParentElement.AppendChild(xamlParagraph);
        }

        // Need to return last processed node
        return lastNodeProcessed;
    }

    // .............................................................
    //
    // Inline Elements
    //
    // .............................................................

    private static void AddInline(XmlElement xamlParentElement, XmlNode? htmlNode, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        if (htmlNode is XmlComment comment)
        {
            DefineInlineFragmentParent(comment, xamlParentElement);
        }
        else if (htmlNode is XmlText)
        {
            AddTextRun(xamlParentElement, htmlNode!.Value!);
        }
        else if (htmlNode is XmlElement htmlElement)
        {

            // Check whether this is an html element
            if (htmlElement.NamespaceURI != HtmlParser.XhtmlNamespace)
            {
                return; // Skip non-html elements
            }

            // Identify element name
            var htmlElementName = htmlElement.LocalName.ToLower(CultureInfo.InvariantCulture);

            // Put source element to the stack
            sourceContext.Add(htmlElement);

            switch (htmlElementName)
            {
                case "a":
                    AddHyperlink(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                    break;
                case "br":
                case "hr":
                    AddBreak(xamlParentElement, htmlElementName);
                    break;
                default:
                    if (HtmlSchema.IsInlineElement(htmlElementName) || HtmlSchema.IsBlockElement(htmlElementName))
                    {
                        // Note: actually we do not expect block elements here,
                        // but if it happens to be here, we will treat it as a Span.

                        AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                    }
                    break;
            }
            // Ignore all other elements non-(block/inline/image)

            // Remove the element from the stack
            Debug.Assert(sourceContext.Count > 0 && sourceContext[^1] == htmlElement);
            sourceContext.RemoveAt(sourceContext.Count - 1);
        }
    }

    private static void AddSpanOrRun(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        // Decide what XAML element to use for this inline element.
        // Check whether it contains any nested inlines
        var elementHasChildren = false;
        for (var htmlNode = htmlElement.FirstChild; htmlNode != null; htmlNode = htmlNode.NextSibling)
        {
            if (htmlNode is XmlElement element)
            {
                var htmlChildName = element.LocalName.ToLower(CultureInfo.InvariantCulture);
                if (HtmlSchema.IsInlineElement(htmlChildName) || HtmlSchema.IsBlockElement(htmlChildName) ||
                    htmlChildName == "img" || htmlChildName == "br" || htmlChildName == "hr")
                {
                    elementHasChildren = true;
                    break;
                }
            }
        }

        var xamlElementName = elementHasChildren ? XamlSpan : XamlRun;

        // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
        var currentProperties = GetElementProperties(htmlElement, inheritedProperties, out var localProperties, stylesheet, sourceContext);

        // Create a XAML element corresponding to this html element
        var xamlElement = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/xamlElementName, XamlNamespace);
        ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/false);

        // Recurse into element subtree
        for (var htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling)
        {
            AddInline(xamlElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
        }

        // Add the new element to the parent.
        _ = xamlParentElement.AppendChild(xamlElement);
    }

    // Adds a text run to a xaml tree
    private static void AddTextRun(XmlElement xamlElement, string textData)
    {
        // Remove control characters
        for (var i = 0; i < textData.Length; i++)
        {
            if (char.IsControl(textData[i]))
            {
                textData = textData.Remove(i--, 1);  // decrement i to compensate for character removal
            }
        }

        // Replace No-Breaks by spaces (160 is a code of &nbsp; entity in html)
        //  This is a work around the bug in Avalon which does not render nbsp.
        textData = textData.Replace((char)160, ' ');

        if (textData.Length > 0)
        {
            _ = xamlElement.AppendChild(xamlElement.OwnerDocument.CreateTextNode(textData));
        }
    }

    private static void AddHyperlink(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        // Convert href attribute into NavigateUri and TargetName
        var href = GetAttribute(htmlElement, "href");
        if (href == null)
        {
            // When href attribute is missing - ignore the hyperlink
            AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
        }
        else
        {
            // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
            var currentProperties = GetElementProperties(htmlElement, inheritedProperties, out var localProperties, stylesheet, sourceContext);

            // Create a XAML element corresponding to this html element
            var xamlElement = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/XamlHyperlink, XamlNamespace);
            ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/false);

            var hrefParts = href.Split('#');
            if (hrefParts.Length > 0 && hrefParts[0].Trim().Length > 0)
            {
                xamlElement.SetAttribute(XamlHyperlinkNavigateUri, hrefParts[0].Trim());
            }
            if (hrefParts.Length == 2 && hrefParts[1].Trim().Length > 0)
            {
                xamlElement.SetAttribute(XamlHyperlinkTargetName, hrefParts[1].Trim());
            }

            // Recurse into element subtree
            for (var htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling)
            {
                AddInline(xamlElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
            }

            // Add the new element to the parent.
            _ = xamlParentElement.AppendChild(xamlElement);
        }
    }

    // Stores a parent xaml element for the case when selected fragment is inline.
    private static XmlElement? _inlineFragmentParentElement;

    // Called when html comment is encountered to store a parent element
    // for the case when the fragment is inline - to extract it to a separate
    // Span wrapper after the conversion.
    private static void DefineInlineFragmentParent(XmlComment htmlComment, XmlElement? xamlParentElement)
    {
        if (htmlComment.Value == "StartFragment")
        {
            _inlineFragmentParentElement = xamlParentElement;
        }
        else if (htmlComment.Value == "EndFragment" && _inlineFragmentParentElement == null && xamlParentElement != null)
        {
            // Normally this cannot happen if comments produced by correct copying code
            // in Word or IE, but when it is produced manually then fragment boundary
            // markers can be inconsistent. In this case StartFragment takes precedence,
            // but if it is not set, then we get the value from EndFragment marker.
            _inlineFragmentParentElement = xamlParentElement;
        }
    }

    // Extracts a content of an element stored as InlineFragmentParentElement
    // into a separate Span wrapper.
    // Note: when selected content does not cross paragraph boundaries,
    // the fragment is marked within
    private static XmlElement ExtractInlineFragment(XmlElement xamlFlowDocumentElement)
    {
        if (_inlineFragmentParentElement != null)
        {
            if (_inlineFragmentParentElement.LocalName == XamlSpan)
            {
                xamlFlowDocumentElement = _inlineFragmentParentElement;
            }
            else
            {
                xamlFlowDocumentElement = xamlFlowDocumentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/XamlSpan, XamlNamespace);
                while (_inlineFragmentParentElement.FirstChild != null)
                {
                    var copyNode = _inlineFragmentParentElement.FirstChild;
                    _ = _inlineFragmentParentElement.RemoveChild(copyNode);
                    _ = xamlFlowDocumentElement.AppendChild(copyNode);
                }
            }
        }

        return xamlFlowDocumentElement;
    }

    // .............................................................
    //
    // Lists
    //
    // .............................................................

    /// <summary>
    /// Converts Html ul or ol element into Xaml list element. During conversion if the ul/ol element has any children 
    /// that are not li elements, they are ignored and not added to the list element
    /// </summary>
    /// <param name="xamlParentElement">
    /// XmlElement representing Xaml parent to which the converted element should be added
    /// </param>
    /// <param name="htmlListElement">
    /// XmlElement representing Html ul/ol element to be converted
    /// </param>
    /// <param name="inheritedProperties">
    /// properties inherited from parent context
    /// </param>
    /// <param name="stylesheet"></param>
    /// <param name="sourceContext"></param>
    private static void AddList(XmlElement xamlParentElement, XmlElement htmlListElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        var htmlListElementName = htmlListElement.LocalName.ToLower(CultureInfo.InvariantCulture);

        var currentProperties = GetElementProperties(htmlListElement, inheritedProperties, out var localProperties, stylesheet, sourceContext);

        // Create Xaml List element
        var xamlListElement = xamlParentElement.OwnerDocument.CreateElement(null, XamlList, XamlNamespace);

        // Set default list markers
        if (htmlListElementName == "ol")
        {
            // Ordered list
            xamlListElement.SetAttribute(XamlListMarkerStyle, XamlListMarkerStyleDecimal);
        }
        else
        {
            // Unordered list - all elements other than OL treated as unordered lists
            xamlListElement.SetAttribute(XamlListMarkerStyle, XamlListMarkerStyleDisc);
        }

        // Apply local properties to list to set marker attribute if specified
        ApplyLocalProperties(xamlListElement, localProperties, /*isBlock:*/true);

        // Recurse into list subtree
        for (var htmlChildNode = htmlListElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling)
        {
            if (htmlChildNode is XmlElement element && htmlChildNode.LocalName.Equals("li", StringComparison.CurrentCultureIgnoreCase))
            {
                sourceContext.Add(element);
                AddListItem(xamlListElement, element, currentProperties, stylesheet, sourceContext);
                Debug.Assert(sourceContext.Count > 0 && sourceContext[^1] == htmlChildNode);
                sourceContext.RemoveAt(sourceContext.Count - 1);
            }
            else
            {
                // Not an li element. Add it to previous ListBoxItem
                //  We need to append the content to the end
                // of a previous list item.
            }
        }

        // Add the List element to xaml tree - if it is not empty
        if (xamlListElement.HasChildNodes)
        {
            _ = xamlParentElement.AppendChild(xamlListElement);
        }
    }

    /// <summary>
    /// If li items are found without a parent ul/ol element in Html string, creates xamlListElement as their parent and adds
    /// them to it. If the previously added node to the same xamlParentElement was a List, adds the elements to that list.
    /// Otherwise, we create a new xamlListElement and add them to it. Elements are added as long as li elements appear sequentially.
    /// The first non-li or text node stops the addition.
    /// </summary>
    /// <param name="xamlParentElement">
    /// Parent element for the list
    /// </param>
    /// <param name="htmlLIElement">
    /// Start Html li element without parent list
    /// </param>
    /// <param name="inheritedProperties">
    /// Properties inherited from parent context
    /// </param>
		/// <param name="sourceContext"></param>
		/// <param name="stylesheet"></param>
    /// <returns>
    /// XmlNode representing the first non-li node in the input after one or more li's have been processed.
    /// </returns>
    private static XmlElement? AddOrphanListItems(XmlElement xamlParentElement, XmlElement htmlLIElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        Debug.Assert(htmlLIElement.LocalName.Equals("li", StringComparison.CurrentCultureIgnoreCase));

        XmlElement? lastProcessedListItemElement = null;

        // Find out the last element attached to the xamlParentElement, which is the previous sibling of this node
        var xamlListItemElementPreviousSibling = xamlParentElement.LastChild;
        XmlElement xamlListElement;
        if (xamlListItemElementPreviousSibling != null && xamlListItemElementPreviousSibling.LocalName == XamlList)
        {
            // Previously added Xaml element was a list. We will add the new li to it
            xamlListElement = (XmlElement)xamlListItemElementPreviousSibling;
        }
        else
        {
            // No list element near. Create our own.
            xamlListElement = xamlParentElement.OwnerDocument.CreateElement(null, XamlList, XamlNamespace);
            _ = xamlParentElement.AppendChild(xamlListElement);
        }

        XmlNode? htmlChildNode = htmlLIElement;
        var htmlChildNodeName = htmlChildNode.LocalName.ToLower(CultureInfo.InvariantCulture);

        // Add li elements to the parent xamlListElement we created as long as they appear sequentially
        // Use properties inherited from xamlParentElement for context 
        while (htmlChildNode != null && htmlChildNodeName == "li")
        {
            AddListItem(xamlListElement, (XmlElement)htmlChildNode, inheritedProperties, stylesheet, sourceContext);
            lastProcessedListItemElement = (XmlElement)htmlChildNode;
            htmlChildNode = htmlChildNode.NextSibling;
            htmlChildNodeName = htmlChildNode?.LocalName.ToLower(CultureInfo.InvariantCulture);
        }

        return lastProcessedListItemElement;
    }

    /// <summary>
    /// Converts htmlLIElement into Xaml ListItem element, and appends it to the parent xamlListElement
    /// </summary>
    /// <param name="xamlListElement">
    /// XmlElement representing Xaml List element to which the converted td/th should be added
    /// </param>
    /// <param name="htmlLIElement">
    /// XmlElement representing Html li element to be converted
    /// </param>
    /// <param name="inheritedProperties">
    /// Properties inherited from parent context
    /// </param>
		/// <param name="sourceContext"></param>
		/// <param name="stylesheet"></param>
		private static void AddListItem(XmlElement xamlListElement, XmlElement htmlLIElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        // Parameter validation
        Debug.Assert(xamlListElement != null);
        Debug.Assert(xamlListElement.LocalName == XamlList);
        Debug.Assert(htmlLIElement != null);
        Debug.Assert(htmlLIElement.LocalName.Equals("li", StringComparison.CurrentCultureIgnoreCase));
        Debug.Assert(inheritedProperties != null);
        var currentProperties = GetElementProperties(htmlLIElement, inheritedProperties, out _, stylesheet, sourceContext);

        var xamlListItemElement = xamlListElement.OwnerDocument.CreateElement(null, XamlListItem, XamlNamespace);

        // Process children of the ListItem
        for (var htmlChildNode = htmlLIElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode?.NextSibling)
        {
            htmlChildNode = AddBlock(xamlListItemElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
        }

        // Add resulting ListBoxItem to a xaml parent
        _ = xamlListElement.AppendChild(xamlListItemElement);
    }

    // .............................................................
    //
    // Tables
    //
    // .............................................................

    /// <summary>
    /// Converts htmlTableElement to a Xaml Table element. Adds tbody elements if they are missing so
    /// that a resulting Xaml Table element is properly formed.
    /// </summary>
    /// <param name="xamlParentElement">
    /// Parent xaml element to which a converted table must be added.
    /// </param>
    /// <param name="htmlTableElement">
    /// XmlElement reprsenting the Html table element to be converted
    /// </param>
    /// <param name="inheritedProperties">
    /// Hashtable representing properties inherited from parent context. 
    /// </param>
		/// <param name="sourceContext"></param>
		/// <param name="stylesheet"></param>
    private static void AddTable(XmlElement xamlParentElement, XmlElement htmlTableElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        // Parameter validation
        Debug.Assert(htmlTableElement.LocalName.Equals("table", StringComparison.CurrentCultureIgnoreCase));
        Debug.Assert(xamlParentElement != null);
        Debug.Assert(inheritedProperties != null);

        // Create current properties to be used by children as inherited properties, set local properties
        var currentProperties = GetElementProperties(htmlTableElement, inheritedProperties, out _, stylesheet, sourceContext);

        // Check if the table contains only one cell - we want to take only its content
        var singleCell = GetCellFromSingleCellTable(htmlTableElement);

        if (singleCell != null)
        {
            //  Need to push skipped table elements onto sourceContext
            sourceContext.Add(singleCell);

            // Add the cell's content directly to parent
            for (var htmlChildNode = singleCell.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode?.NextSibling)
            {
                htmlChildNode = AddBlock(xamlParentElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
            }

            Debug.Assert(sourceContext.Count > 0 && sourceContext[^1] == singleCell);
            sourceContext.RemoveAt(sourceContext.Count - 1);
        }
        else
        {
            // Create xamlTableElement
            var xamlTableElement = xamlParentElement.OwnerDocument.CreateElement(null, XamlTable, XamlNamespace);

            // Analyze table structure for column widths and rowspan attributes
            var columnStarts = AnalyzeTableStructure(htmlTableElement);

            // Process COLGROUP & COL elements
            AddColumnInformation(htmlTableElement, xamlTableElement, columnStarts, currentProperties, stylesheet, sourceContext);

            // Process table body - TBODY and TR elements
            var htmlChildNode = htmlTableElement.FirstChild;

            while (htmlChildNode != null)
            {
                var htmlChildName = htmlChildNode.LocalName.ToLower(CultureInfo.InvariantCulture);

                // Process the element
                if (htmlChildName is "tbody" or "thead" or "tfoot")
                {
                    //  Add more special processing for TableHeader and TableFooter
                    var xamlTableBodyElement = xamlTableElement.OwnerDocument.CreateElement(null, XamlTableRowGroup, XamlNamespace);
                    _ = xamlTableElement.AppendChild(xamlTableBodyElement);

                    sourceContext.Add((XmlElement)htmlChildNode);

                    // Get properties of Html tbody element
                    var tbodyElementCurrentProperties = GetElementProperties((XmlElement)htmlChildNode, currentProperties, out _, stylesheet, sourceContext);

                    // Process children of htmlChildNode, which is tbody, for tr elements
                    _ = AddTableRowsToTableBody(xamlTableBodyElement, htmlChildNode.FirstChild, tbodyElementCurrentProperties, columnStarts, stylesheet, sourceContext);
                    if (xamlTableBodyElement.HasChildNodes)
                    {
                        _ = xamlTableElement.AppendChild(xamlTableBodyElement);
                        // else: if there is no TRs in this TBody, we simply ignore it
                    }

                    Debug.Assert(sourceContext.Count > 0 && sourceContext[^1] == htmlChildNode);
                    sourceContext.RemoveAt(sourceContext.Count - 1);

                    htmlChildNode = htmlChildNode.NextSibling;
                }
                else if (htmlChildName == "tr")
                {
                    // Tbody is not present, but tr element is present. Tr is wrapped in tbody
                    var xamlTableBodyElement = xamlTableElement.OwnerDocument.CreateElement(null, XamlTableRowGroup, XamlNamespace);

                    // We use currentProperties of xamlTableElement when adding rows since the tbody element is artificially created and has 
                    // no properties of its own

                    htmlChildNode = AddTableRowsToTableBody(xamlTableBodyElement, htmlChildNode, currentProperties, columnStarts, stylesheet, sourceContext);
                    if (xamlTableBodyElement.HasChildNodes)
                    {
                        _ = xamlTableElement.AppendChild(xamlTableBodyElement);
                    }
                }
                else
                {
                    // Element is not tbody or tr. Ignore it.
                    htmlChildNode = htmlChildNode.NextSibling;
                }
            }

            if (xamlTableElement.HasChildNodes)
            {
                _ = xamlParentElement.AppendChild(xamlTableElement);
            }
        }
    }

    private static XmlElement? GetCellFromSingleCellTable(XmlElement htmlTableElement)
    {
        XmlElement? singleCell = null;

        for (var tableChild = htmlTableElement.FirstChild; tableChild != null; tableChild = tableChild.NextSibling)
        {
            var elementName = tableChild.LocalName.ToLower(CultureInfo.InvariantCulture);
            if (elementName is "tbody" or "thead" or "tfoot")
            {
                if (singleCell != null)
                {
                    return null;
                }
                for (var tbodyChild = tableChild.FirstChild; tbodyChild != null; tbodyChild = tbodyChild.NextSibling)
                {
                    if (tbodyChild.LocalName.Equals("tr", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (singleCell != null)
                        {
                            return null;
                        }
                        for (var trChild = tbodyChild.FirstChild; trChild != null; trChild = trChild.NextSibling)
                        {
                            var cellName = trChild.LocalName.ToLower(CultureInfo.InvariantCulture);
                            if (cellName is "td" or "th")
                            {
                                if (singleCell != null)
                                {
                                    return null;
                                }
                                singleCell = (XmlElement)trChild;
                            }
                        }
                    }
                }
            }
            else if (tableChild.LocalName.Equals("tr", StringComparison.CurrentCultureIgnoreCase))
            {
                if (singleCell != null)
                {
                    return null;
                }
                for (var trChild = tableChild.FirstChild; trChild != null; trChild = trChild.NextSibling)
                {
                    var cellName = trChild.LocalName.ToLower(CultureInfo.InvariantCulture);
                    if (cellName is "td" or "th")
                    {
                        if (singleCell != null)
                        {
                            return null;
                        }
                        singleCell = (XmlElement)trChild;
                    }
                }
            }
        }

        return singleCell;
    }

    /// <summary>
    /// Processes the information about table columns - COLGROUP and COL html elements.
    /// </summary>
    /// <param name="htmlTableElement">
    /// XmlElement representing a source html table.
    /// </param>
    /// <param name="xamlTableElement">
    /// XmlElement repesenting a resulting xaml table.
    /// </param>
    /// <param name="columnStartsAllRows">
    /// Array of doubles - column start coordinates.
    /// Can be null, which means that column size information is not available
    /// and we must use source colgroup/col information.
    /// In case wneh it's not null, we will ignore source colgroup/col information.
    /// </param>
    /// <param name="currentProperties"></param>
    /// <param name="stylesheet"></param>
    /// <param name="sourceContext"></param>
    private static void AddColumnInformation(XmlElement htmlTableElement, XmlElement xamlTableElement, ArrayList? columnStartsAllRows, Hashtable currentProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        // Add column information
        if (columnStartsAllRows != null)
        {
            // We have consistent information derived from table cells; use it
            // The last element in columnStarts represents the end of the table
            for (var columnIndex = 0; columnIndex < columnStartsAllRows.Count - 1; columnIndex++)
            {
                XmlElement xamlColumnElement;

                xamlColumnElement = xamlTableElement.OwnerDocument.CreateElement(null, XamlTableColumn, XamlNamespace);
                xamlColumnElement.SetAttribute(XamlWidth, ((double)columnStartsAllRows[columnIndex + 1]! - (double)columnStartsAllRows[columnIndex]!).ToString(CultureInfo.InvariantCulture));
                _ = xamlTableElement.AppendChild(xamlColumnElement);
            }
        }
        else
        {
            // We do not have consistent information from table cells.
            // Translate blindly colgroups from html.                
            for (var htmlChildNode = htmlTableElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling)
            {
                if (htmlChildNode.LocalName.Equals("colgroup", StringComparison.CurrentCultureIgnoreCase))
                {
                    AddTableColumnGroup(xamlTableElement, (XmlElement)htmlChildNode, currentProperties, stylesheet, sourceContext);
                }
                else if (htmlChildNode.LocalName.Equals("col", StringComparison.CurrentCultureIgnoreCase))
                {
                    AddTableColumn(xamlTableElement, (XmlElement)htmlChildNode, currentProperties, stylesheet, sourceContext);
                }
                else if (htmlChildNode is XmlElement)
                {
                    // Some element which belongs to table body. Stop column loop.
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Converts htmlColgroupElement into Xaml TableColumnGroup element, and appends it to the parent
    /// xamlTableElement
    /// </summary>
    /// <param name="xamlTableElement">
    /// XmlElement representing Xaml Table element to which the converted column group should be added
    /// </param>
    /// <param name="htmlColgroupElement">
    /// XmlElement representing Html colgroup element to be converted
		/// </param>
    /// <param name="inheritedProperties">
    /// Properties inherited from parent context
    /// </param>
		/// <param name="sourceContext"></param>
		/// <param name="stylesheet"></param>
    private static void AddTableColumnGroup(XmlElement xamlTableElement, XmlElement htmlColgroupElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        var currentProperties = GetElementProperties(htmlColgroupElement, inheritedProperties, out _, stylesheet, sourceContext);

        // Process children of colgroup. Colgroup may contain only col elements.
        for (var htmlNode = htmlColgroupElement.FirstChild; htmlNode != null; htmlNode = htmlNode.NextSibling)
        {
            if (htmlNode is XmlElement element && htmlNode.LocalName.Equals("col", StringComparison.CurrentCultureIgnoreCase))
            {
                AddTableColumn(xamlTableElement, element, currentProperties, stylesheet, sourceContext);
            }
        }
    }

    /// <summary>
    /// Converts htmlColElement into Xaml TableColumn element, and appends it to the parent
    /// xamlTableColumnGroupElement
    /// </summary>
    /// <param name="xamlTableElement"></param>
    /// <param name="htmlColElement">
    /// XmlElement representing Html col element to be converted
    /// </param>
    /// <param name="inheritedProperties">
    /// properties inherited from parent context
    /// </param>
    /// <param name="stylesheet"></param>
    /// <param name="sourceContext"></param>
    private static void AddTableColumn(XmlElement xamlTableElement, XmlElement htmlColElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        _ = GetElementProperties(htmlColElement, inheritedProperties, out _, stylesheet, sourceContext);

        var xamlTableColumnElement = xamlTableElement.OwnerDocument.CreateElement(null, XamlTableColumn, XamlNamespace);

        // Col is an empty element, with no subtree 
        _ = xamlTableElement.AppendChild(xamlTableColumnElement);
    }

    /// <summary>
    /// Adds TableRow elements to xamlTableBodyElement. The rows are converted from Html tr elements that
    /// may be the children of an Html tbody element or an Html table element with tbody missing
    /// </summary>
    /// <param name="xamlTableBodyElement">
    /// XmlElement representing Xaml TableRowGroup element to which the converted rows should be added
    /// </param>
    /// <param name="htmlTRStartNode">
    /// XmlElement representing the first tr child of the tbody element to be read
    /// </param>
    /// <param name="currentProperties">
    /// Hashtable representing current properties of the tbody element that are generated and applied in the
    /// AddTable function; to be used as inheritedProperties when adding tr elements
    /// </param>
    /// <param name="columnStarts"></param>
    /// <param name="stylesheet"></param>
    /// <param name="sourceContext"></param>
    /// <returns>
    /// XmlNode representing the current position of the iterator among tr elements
    /// </returns>
    private static XmlNode? AddTableRowsToTableBody(XmlElement xamlTableBodyElement, XmlNode? htmlTRStartNode, Hashtable currentProperties, ArrayList? columnStarts, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        // Parameter validation
        Debug.Assert(xamlTableBodyElement.LocalName == XamlTableRowGroup);
        Debug.Assert(currentProperties != null);

        // Initialize child node for iteratimg through children to the first tr element
        var htmlChildNode = htmlTRStartNode;
        ArrayList? activeRowSpans = null;
        if (columnStarts != null)
        {
            activeRowSpans = [];
            InitializeActiveRowSpans(activeRowSpans, columnStarts.Count);
        }

        while (htmlChildNode != null && !htmlChildNode.LocalName.Equals("tbody", StringComparison.CurrentCultureIgnoreCase))
        {
            if (htmlChildNode.LocalName.Equals("tr", StringComparison.CurrentCultureIgnoreCase))
            {
                var xamlTableRowElement = xamlTableBodyElement.OwnerDocument.CreateElement(null, XamlTableRow, XamlNamespace);

                sourceContext.Add((XmlElement)htmlChildNode);

                // Get tr element properties
                var trElementCurrentProperties = GetElementProperties((XmlElement)htmlChildNode, currentProperties, out _, stylesheet, sourceContext);

                _ = AddTableCellsToTableRow(xamlTableRowElement, htmlChildNode.FirstChild, trElementCurrentProperties, columnStarts, activeRowSpans, stylesheet, sourceContext);
                if (xamlTableRowElement.HasChildNodes)
                {
                    _ = xamlTableBodyElement.AppendChild(xamlTableRowElement);
                }

                Debug.Assert(sourceContext.Count > 0 && sourceContext[^1] == htmlChildNode);
                sourceContext.RemoveAt(sourceContext.Count - 1);

                // Advance
                htmlChildNode = htmlChildNode.NextSibling;

            }
            else if (htmlChildNode.LocalName.Equals("td", StringComparison.CurrentCultureIgnoreCase))
            {
                // Tr element is not present. We create one and add td elements to it
                var xamlTableRowElement = xamlTableBodyElement.OwnerDocument.CreateElement(null, XamlTableRow, XamlNamespace);

                // This is incorrect formatting and the column starts should not be set in this case
                Debug.Assert(columnStarts == null);

                htmlChildNode = AddTableCellsToTableRow(xamlTableRowElement, htmlChildNode, currentProperties, columnStarts, activeRowSpans, stylesheet, sourceContext);
                if (xamlTableRowElement.HasChildNodes)
                {
                    _ = xamlTableBodyElement.AppendChild(xamlTableRowElement);
                }
            }
            else
            {
                // Not a tr or td  element. Ignore it.
                htmlChildNode = htmlChildNode.NextSibling;
            }
        }
        return htmlChildNode;
    }

    /// <summary>
    /// Adds TableCell elements to xamlTableRowElement.
    /// </summary>
    /// <param name="xamlTableRowElement">
    /// XmlElement representing Xaml TableRow element to which the converted cells should be added
    /// </param>
    /// <param name="htmlTDStartNode">
    /// XmlElement representing the child of tr or tbody element from which we should start adding td elements
    /// </param>
    /// <param name="currentProperties">
    /// properties of the current html tr element to which cells are to be added
    /// </param>
		/// <param name="activeRowSpans"></param>
		/// <param name="columnStarts"></param>
		/// <param name="sourceContext"></param>
		/// <param name="stylesheet"></param>
    /// <returns>
    /// XmlElement representing the current position of the iterator among the children of the parent Html tbody/tr element
    /// </returns>
    private static XmlNode? AddTableCellsToTableRow(XmlElement xamlTableRowElement, XmlNode? htmlTDStartNode, Hashtable currentProperties, ArrayList? columnStarts, ArrayList? activeRowSpans, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        // parameter validation
        Debug.Assert(xamlTableRowElement.LocalName == XamlTableRow);
        Debug.Assert(currentProperties != null);
        if (columnStarts != null)
        {
            Debug.Assert(activeRowSpans?.Count == columnStarts.Count);
        }

        var htmlChildNode = htmlTDStartNode;
        var columnIndex = 0;
        while (htmlChildNode != null && !htmlChildNode.LocalName.Equals("tr", StringComparison.CurrentCultureIgnoreCase) && !htmlChildNode.LocalName.Equals("tbody", StringComparison.CurrentCultureIgnoreCase) && !htmlChildNode.LocalName.Equals("thead", StringComparison.CurrentCultureIgnoreCase) && !htmlChildNode.LocalName.Equals("tfoot", StringComparison.CurrentCultureIgnoreCase))
        {
            if (htmlChildNode.LocalName.ToLower(CultureInfo.InvariantCulture) is "td" or "th")
            {
                var xamlTableCellElement = xamlTableRowElement.OwnerDocument.CreateElement(null, XamlTableCell, XamlNamespace);

                sourceContext.Add((XmlElement)htmlChildNode);
                var tdElementCurrentProperties = GetElementProperties((XmlElement)htmlChildNode, currentProperties, out _, stylesheet, sourceContext);

                ApplyPropertiesToTableCellElement((XmlElement)htmlChildNode, xamlTableCellElement);

                if (activeRowSpans is not null && columnStarts != null)
                {
                    Debug.Assert(columnIndex < columnStarts.Count - 1);
                    while (columnIndex < activeRowSpans.Count && (int)activeRowSpans[columnIndex]! > 0)
                    {
                        activeRowSpans[columnIndex] = (int)activeRowSpans[columnIndex]! - 1;
                        Debug.Assert((int)activeRowSpans[columnIndex]! >= 0);
                        columnIndex++;
                    }
                    Debug.Assert(columnIndex < columnStarts.Count - 1);
                    var columnWidth = GetColumnWidth((XmlElement)htmlChildNode);
                    var columnSpan = CalculateColumnSpan(columnIndex, columnWidth, columnStarts);
                    var rowSpan = GetRowSpan((XmlElement)htmlChildNode);

                    // Column cannot have no span
                    Debug.Assert(columnSpan > 0);
                    Debug.Assert(columnIndex + columnSpan < columnStarts.Count);

                    xamlTableCellElement.SetAttribute(XamlTableCellColumnSpan, columnSpan.ToString(CultureInfo.InvariantCulture));

                    // Apply row span
                    for (var spannedColumnIndex = columnIndex; spannedColumnIndex < columnIndex + columnSpan; spannedColumnIndex++)
                    {
                        Debug.Assert(spannedColumnIndex < activeRowSpans.Count);
                        activeRowSpans[spannedColumnIndex] = rowSpan - 1;
                        Debug.Assert((int)activeRowSpans[spannedColumnIndex]! >= 0);
                    }

                    columnIndex += columnSpan;
                }

                AddDataToTableCell(xamlTableCellElement, htmlChildNode.FirstChild, tdElementCurrentProperties, stylesheet, sourceContext);
                if (xamlTableCellElement.HasChildNodes)
                {
                    _ = xamlTableRowElement.AppendChild(xamlTableCellElement);
                }

                Debug.Assert(sourceContext.Count > 0 && sourceContext[^1] == htmlChildNode);
                sourceContext.RemoveAt(sourceContext.Count - 1);

                htmlChildNode = htmlChildNode.NextSibling;
            }
            else
            {
                htmlChildNode = htmlChildNode.NextSibling;
            }
        }
        return htmlChildNode;
    }

    /// <summary>
    /// adds table cell data to xamlTableCellElement
    /// </summary>
    /// <param name="xamlTableCellElement">
    /// XmlElement representing Xaml TableCell element to which the converted data should be added
    /// </param>
    /// <param name="htmlDataStartNode">
    /// XmlElement representing the start element of data to be added to xamlTableCellElement
    /// </param>
    /// <param name="currentProperties">
    /// Current properties for the html td/th element corresponding to xamlTableCellElement
    /// </param>
		/// <param name="sourceContext"></param>
		/// <param name="stylesheet"></param>
    private static void AddDataToTableCell(XmlElement xamlTableCellElement, XmlNode? htmlDataStartNode, Hashtable currentProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        // Parameter validation
        Debug.Assert(xamlTableCellElement.LocalName == XamlTableCell);
        Debug.Assert(currentProperties != null);

        for (var htmlChildNode = htmlDataStartNode; htmlChildNode != null; htmlChildNode = htmlChildNode?.NextSibling)
        {
            // Process a new html element and add it to the td element
            htmlChildNode = AddBlock(xamlTableCellElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
        }
    }

    /// <summary>
    /// Performs a parsing pass over a table to read information about column width and rowspan attributes. This information
    /// is used to determine the starting point of each column. 
    /// </summary>
    /// <param name="htmlTableElement">
    /// XmlElement representing Html table whose structure is to be analyzed
    /// </param>
    /// <returns>
    /// ArrayList of type double which contains the function output. If analysis is successful, this ArrayList contains
    /// all the points which are the starting position of any column in the table, ordered from left to right.
    /// In case if analisys was impossible we return null.
    /// </returns>
    private static ArrayList? AnalyzeTableStructure(XmlElement htmlTableElement)
    {
        // Parameter validation
        Debug.Assert(htmlTableElement.LocalName.Equals("table", StringComparison.CurrentCultureIgnoreCase));
        if (!htmlTableElement.HasChildNodes)
        {
            return [];
        }

        var columnWidthsAvailable = true;

        var columnStarts = new ArrayList();
        var activeRowSpans = new ArrayList();
        Debug.Assert(columnStarts.Count == activeRowSpans.Count);

        var htmlChildNode = htmlTableElement.FirstChild;
        double tableWidth = 0;  // Keep track of table width which is the width of its widest row

        // Analyze tbody and tr elements
        while (htmlChildNode != null && columnWidthsAvailable)
        {
            Debug.Assert(columnStarts.Count == activeRowSpans.Count);

            switch (htmlChildNode.LocalName.ToLower(CultureInfo.InvariantCulture))
            {
                case "tbody":
                    // Tbody element, we should analyze its children for trows
                    var tbodyWidth = AnalyzeTbodyStructure((XmlElement)htmlChildNode, columnStarts, activeRowSpans);
                    if (tbodyWidth > tableWidth)
                    {
                        // Table width must be increased to supported newly added wide row
                        tableWidth = tbodyWidth;
                    }
                    else if (tbodyWidth == 0)
                    {
                        // Tbody analysis may return 0, probably due to unprocessable format. 
                        // We should also fail.
                        columnWidthsAvailable = false; // interrupt the analisys
                    }
                    break;
                case "tr":
                    // Table row. Analyze column structure within row directly
                    var trWidth = AnalyzeTRStructure((XmlElement)htmlChildNode, columnStarts, activeRowSpans);
                    if (trWidth > tableWidth)
                    {
                        tableWidth = trWidth;
                    }
                    else if (trWidth == 0)
                    {
                        columnWidthsAvailable = false; // interrupt the analisys
                    }
                    break;
                case "td":
                    columnWidthsAvailable = false; // interrupt the analisys
                    break;
                default:
                    // Element should not occur directly in table. Ignore it.
                    break;
            }

            htmlChildNode = htmlChildNode.NextSibling;
        }

        if (columnWidthsAvailable)
        {
            // Add an item for whole table width
            _ = columnStarts.Add(tableWidth);
            VerifyColumnStartsAscendingOrder(columnStarts);
        }
        else
        {
            columnStarts = null;
        }

        return columnStarts;
    }

    /// <summary>
    /// Performs a parsing pass over a tbody to read information about column width and rowspan attributes. Information read about width
    /// attributes is stored in the reference ArrayList parameter columnStarts, which contains a list of all starting
    /// positions of all columns in the table, ordered from left to right. Row spans are taken into consideration when 
    /// computing column starts
    /// </summary>
    /// <param name="htmlTbodyElement">
    /// XmlElement representing Html tbody whose structure is to be analyzed
    /// </param>
    /// <param name="columnStarts">
    /// ArrayList of type double which contains the function output. If analysis fails, this parameter is set to null
    /// </param>
		/// <param name="activeRowSpans"></param>
    /// <returns>
    /// Calculated width of a tbody.
    /// In case of non-analizable column width structure return 0;
    /// </returns>
    private static double AnalyzeTbodyStructure(XmlElement htmlTbodyElement, ArrayList columnStarts, ArrayList activeRowSpans)
    {
        // Parameter validation
        Debug.Assert(htmlTbodyElement.LocalName.Equals("tbody", StringComparison.CurrentCultureIgnoreCase));
        Debug.Assert(columnStarts != null);

        double tbodyWidth = 0;
        var columnWidthsAvailable = true;

        if (!htmlTbodyElement.HasChildNodes)
        {
            return tbodyWidth;
        }

        // Set active row spans to 0 - thus ignoring row spans crossing tbody boundaries
        ClearActiveRowSpans(activeRowSpans);

        var htmlChildNode = htmlTbodyElement.FirstChild;

        // Analyze tr elements
        while (htmlChildNode != null && columnWidthsAvailable)
        {
            switch (htmlChildNode.LocalName.ToLower(CultureInfo.InvariantCulture))
            {
                case "tr":
                    var trWidth = AnalyzeTRStructure((XmlElement)htmlChildNode, columnStarts, activeRowSpans);
                    if (trWidth > tbodyWidth)
                    {
                        tbodyWidth = trWidth;
                    }
                    break;
                case "td":
                    columnWidthsAvailable = false; // interrupt the analisys
                    break;
                default:
                    break;
            }
            htmlChildNode = htmlChildNode.NextSibling;
        }

        // Set active row spans to 0 - thus ignoring row spans crossing tbody boundaries
        ClearActiveRowSpans(activeRowSpans);

        return columnWidthsAvailable ? tbodyWidth : 0;
    }

    /// <summary>
    /// Performs a parsing pass over a tr element to read information about column width and rowspan attributes.  
    /// </summary>
    /// <param name="htmlTRElement">
    /// XmlElement representing Html tr element whose structure is to be analyzed
    /// </param>
    /// <param name="columnStarts">
    /// ArrayList of type double which contains the function output. If analysis is successful, this ArrayList contains
    /// all the points which are the starting position of any column in the tr, ordered from left to right. If analysis fails,
    /// the ArrayList is set to null
    /// </param>
    /// <param name="activeRowSpans">
    /// ArrayList representing all columns currently spanned by an earlier row span attribute. These columns should
    /// not be used for data in this row. The ArrayList actually contains notation for all columns in the table, if the
    /// active row span is set to 0 that column is not presently spanned but if it is > 0 the column is presently spanned
    /// </param>
    private static double AnalyzeTRStructure(XmlElement htmlTRElement, ArrayList columnStarts, ArrayList activeRowSpans)
    {
        double columnWidth;

        // Parameter validation
        Debug.Assert(htmlTRElement.LocalName.Equals("tr", StringComparison.CurrentCultureIgnoreCase));
        Debug.Assert(columnStarts != null);
        Debug.Assert(activeRowSpans != null);
        Debug.Assert(columnStarts.Count == activeRowSpans.Count);

        if (!htmlTRElement.HasChildNodes)
        {
            return 0;
        }

        var columnWidthsAvailable = true;

        double columnStart = 0; // starting position of current column
        var htmlChildNode = htmlTRElement.FirstChild;
        var columnIndex = 0;

        // Skip spanned columns to get to real column start
        if (columnIndex < activeRowSpans.Count)
        {
            Debug.Assert((double)columnStarts[columnIndex]! >= columnStart);
            if ((double)columnStarts[columnIndex]! == columnStart)
            {
                // The new column may be in a spanned area
                while (columnIndex < activeRowSpans.Count && (int)activeRowSpans[columnIndex]! > 0)
                {
                    activeRowSpans[columnIndex] = (int)activeRowSpans[columnIndex]! - 1;
                    Debug.Assert((int)activeRowSpans[columnIndex]! >= 0);
                    columnIndex++;
                    columnStart = (double)columnStarts[columnIndex]!;
                }
            }
        }

        while (htmlChildNode != null && columnWidthsAvailable)
        {
            Debug.Assert(columnStarts.Count == activeRowSpans.Count);

            VerifyColumnStartsAscendingOrder(columnStarts);

            switch (htmlChildNode.LocalName.ToLower(CultureInfo.InvariantCulture))
            {
                case "td":
                    Debug.Assert(columnIndex <= columnStarts.Count);
                    if (columnIndex < columnStarts.Count)
                    {
                        Debug.Assert(columnStart <= (double)columnStarts[columnIndex]!);
                        if (columnStart < (double)columnStarts[columnIndex]!)
                        {
                            columnStarts.Insert(columnIndex, columnStart);
                            // There can be no row spans now - the column data will appear here
                            // Row spans may appear only during the column analysis
                            activeRowSpans.Insert(columnIndex, 0);
                        }
                    }
                    else
                    {
                        // Column start is greater than all previous starts. Row span must still be 0 because
                        // we are either adding after another column of the same row, in which case it should not inherit
                        // the previous column's span. Otherwise we are adding after the last column of some previous
                        // row, and assuming the table widths line up, we should not be spanned by it. If there is
                        // an incorrect tbale structure where a columns starts in the middle of a row span, we do not
                        // guarantee correct output
                        _ = columnStarts.Add(columnStart);
                        _ = activeRowSpans.Add(0);
                    }
                    columnWidth = GetColumnWidth((XmlElement)htmlChildNode);
                    if (columnWidth != -1)
                    {
                        int nextColumnIndex;
                        var rowSpan = GetRowSpan((XmlElement)htmlChildNode);

                        nextColumnIndex = GetNextColumnIndex(columnIndex, columnWidth, columnStarts, activeRowSpans);
                        if (nextColumnIndex != -1)
                        {
                            // Entire column width can be processed without hitting conflicting row span. This means that
                            // column widths line up and we can process them
                            Debug.Assert(nextColumnIndex <= columnStarts.Count);

                            // Apply row span to affected columns
                            for (var spannedColumnIndex = columnIndex; spannedColumnIndex < nextColumnIndex; spannedColumnIndex++)
                            {
                                activeRowSpans[spannedColumnIndex] = rowSpan - 1;
                                Debug.Assert((int)activeRowSpans[spannedColumnIndex]! >= 0);
                            }

                            columnIndex = nextColumnIndex;

                            // Calculate columnsStart for the next cell
                            columnStart += columnWidth;

                            if (columnIndex < activeRowSpans.Count)
                            {
                                Debug.Assert((double)columnStarts[columnIndex]! >= columnStart);
                                if ((double)columnStarts[columnIndex]! == columnStart)
                                {
                                    // The new column may be in a spanned area
                                    while (columnIndex < activeRowSpans.Count && (int)activeRowSpans[columnIndex]! > 0)
                                    {
                                        activeRowSpans[columnIndex] = (int)activeRowSpans[columnIndex]! - 1;
                                        Debug.Assert((int)activeRowSpans[columnIndex]! >= 0);
                                        columnIndex++;
                                        columnStart = (double)columnStarts[columnIndex]!;
                                    }
                                }
                                // else: the new column does not start at the same time as a pre existing column
                                // so we don't have to check it for active row spans, it starts in the middle
                                // of another column which has been checked already by the GetNextColumnIndex function
                            }
                        }
                        else
                        {
                            // Full column width cannot be processed without a pre existing row span.
                            // We cannot analyze widths
                            columnWidthsAvailable = false;
                        }
                    }
                    else
                    {
                        // Incorrect column width, stop processing
                        columnWidthsAvailable = false;
                    }
                    break;
                default:
                    break;
            }

            htmlChildNode = htmlChildNode.NextSibling;
        }

        var trWidth = columnWidthsAvailable ? columnStart : 0;
        // The width of the tr element is the position at which it's last td element ends, which is calculated in
        // the columnStart value after each td element is processed

        return trWidth;
    }

    /// <summary>
    /// Gets row span attribute from htmlTDElement. Returns an integer representing the value of the rowspan attribute.
    /// Default value if attribute is not specified or if it is invalid is 1
    /// </summary>
    /// <param name="htmlTDElement">
    /// Html td element to be searched for rowspan attribute
    /// </param>
    private static int GetRowSpan(XmlElement htmlTDElement)
    {
        string? rowSpanAsString;
        int rowSpan;

        rowSpanAsString = GetAttribute(htmlTDElement, "rowspan");
        if (rowSpanAsString != null)
        {
            if (!int.TryParse(rowSpanAsString, out rowSpan))
            {
                // Ignore invalid value of rowspan; treat it as 1
                rowSpan = 1;
            }
        }
        else
        {
            // No row span, default is 1
            rowSpan = 1;
        }
        return rowSpan;
    }

    /// <summary>
    /// Gets index at which a column should be inseerted into the columnStarts ArrayList. This is
    /// decided by the value columnStart. The columnStarts ArrayList is ordered in ascending order.
    /// Returns an integer representing the index at which the column should be inserted
    /// </summary>
		/// <param name="columnStarts">
    /// Array list representing starting coordinates of all columns in the table
    /// </param>
		/// <param name="columnWidth">
    /// </param>
    /// <param name="columnIndex">
    /// Int representing the current column index. This acts as a clue while finding the insertion index.
    /// If the value of columnStarts at columnIndex is the same as columnStart, then this position alrady exists
    /// in the array and we can jsut return columnIndex.
    /// </param>
		/// <param name="activeRowSpans"></param>
    /// <returns></returns>
    private static int GetNextColumnIndex(int columnIndex, double columnWidth, ArrayList columnStarts, ArrayList activeRowSpans)
    {
        double columnStart;
        int spannedColumnIndex;

        // Parameter validation
        Debug.Assert(columnStarts != null);
        Debug.Assert(0 <= columnIndex && columnIndex <= columnStarts.Count);
        Debug.Assert(columnWidth > 0);

        columnStart = (double)columnStarts[columnIndex]!;
        spannedColumnIndex = columnIndex + 1;

        while (spannedColumnIndex < columnStarts.Count && (double)columnStarts[spannedColumnIndex]! < columnStart + columnWidth && spannedColumnIndex != -1)
        {
            if ((int)activeRowSpans[spannedColumnIndex]! > 0)
            {
                // The current column should span this area, but something else is already spanning it
                // Not analyzable
                spannedColumnIndex = -1;
            }
            else
            {
                spannedColumnIndex++;
            }
        }

        return spannedColumnIndex;
    }


    /// <summary>
    /// Used for clearing activeRowSpans array in the beginning/end of each tbody
    /// </summary>
    /// <param name="activeRowSpans">
    /// ArrayList representing currently active row spans
    /// </param>
    private static void ClearActiveRowSpans(ArrayList activeRowSpans)
    {
        for (var columnIndex = 0; columnIndex < activeRowSpans.Count; columnIndex++)
        {
            activeRowSpans[columnIndex] = 0;
        }
    }

    /// <summary>
    /// Used for initializing activeRowSpans array in the before adding rows to tbody element
    /// </summary>
    /// <param name="activeRowSpans">
    /// ArrayList representing currently active row spans
    /// </param>
    /// <param name="count">
    /// Size to be give to array list
    /// </param>
    private static void InitializeActiveRowSpans(ArrayList activeRowSpans, int count)
    {
        for (var columnIndex = 0; columnIndex < count; columnIndex++)
        {
            _ = activeRowSpans.Add(0);
        }
    }

    private static double GetColumnWidth(XmlElement htmlTDElement)
    {
        string? columnWidthAsString;

        // Get string valkue for the width
        columnWidthAsString = GetAttribute(htmlTDElement, "width");
        columnWidthAsString ??= GetCssAttribute(GetAttribute(htmlTDElement, "style"), "width");

        // We do not allow column width to be 0, if specified as 0 we will fail to record it
        if (!TryGetLengthValue(columnWidthAsString, out var columnWidth) || columnWidth == 0)
        {
            columnWidth = -1;
        }
        return columnWidth;
    }

    /// <summary>
    /// Calculates column span based the column width and the widths of all other columns. Returns an integer representing 
    /// the column span
    /// </summary>
    /// <param name="columnIndex">
    /// Index of the current column
    /// </param>
    /// <param name="columnWidth">
    /// Width of the current column
    /// </param>
    /// <param name="columnStarts">
    /// ArrayList repsenting starting coordinates of all columns
    /// </param>
    private static int CalculateColumnSpan(int columnIndex, double columnWidth, ArrayList columnStarts)
    {
        // Current status of column width. Indicates the amount of width that has been scanned already
        double columnSpanningValue;
        int columnSpanningIndex;
        int columnSpan;
        double subColumnWidth; // Width of the smallest-grain columns in the table

        Debug.Assert(columnStarts != null);
        Debug.Assert(columnIndex < columnStarts.Count - 1);
        Debug.Assert((double)columnStarts[columnIndex]! >= 0);
        Debug.Assert(columnWidth > 0);

        columnSpanningIndex = columnIndex;
        columnSpanningValue = 0;
        while (columnSpanningValue < columnWidth && columnSpanningIndex < columnStarts.Count - 1)
        {
            subColumnWidth = (double)columnStarts[columnSpanningIndex + 1]! - (double)columnStarts[columnSpanningIndex]!;
            Debug.Assert(subColumnWidth > 0);
            columnSpanningValue += subColumnWidth;
            columnSpanningIndex++;
        }

        // Now, we have either covered the width we needed to cover or reached the end of the table, in which
        // case the column spans all the columns until the end
        columnSpan = columnSpanningIndex - columnIndex;
        Debug.Assert(columnSpan > 0);

        return columnSpan;
    }

    /// <summary>
    /// Verifies that values in columnStart, which represent starting coordinates of all columns, are arranged
    /// in ascending order
    /// </summary>
    /// <param name="columnStarts">
    /// ArrayList representing starting coordinates of all columns
    /// </param>
    private static void VerifyColumnStartsAscendingOrder(ArrayList columnStarts)
    {
        Debug.Assert(columnStarts != null);

        double columnStart;

        columnStart = -0.01;

        for (var columnIndex = 0; columnIndex < columnStarts.Count; columnIndex++)
        {
            Debug.Assert(columnStart < (double)columnStarts[columnIndex]!);
            columnStart = (double)columnStarts[columnIndex]!;
        }
    }

    // .............................................................
    //
    // Attributes and Properties
    //
    // .............................................................

    /// <summary>
    /// Analyzes local properties of Html element, converts them into Xaml equivalents, and applies them to xamlElement
    /// </summary>
    /// <param name="xamlElement">
    /// XmlElement representing Xaml element to which properties are to be applied
    /// </param>
    /// <param name="localProperties">
    /// Hashtable representing local properties of Html element that is converted into xamlElement
    /// </param>
		/// <param name="isBlock"></param>
    private static void ApplyLocalProperties(XmlElement xamlElement, Hashtable localProperties, bool isBlock)
    {
        var marginSet = false;
        var marginTop = "0";
        var marginBottom = "0";
        var marginLeft = "0";
        var marginRight = "0";

        var paddingSet = false;
        var paddingTop = "0";
        var paddingBottom = "0";
        var paddingLeft = "0";
        var paddingRight = "0";

        string? borderColor = null;

        var borderThicknessSet = false;
        var borderThicknessTop = "0";
        var borderThicknessBottom = "0";
        var borderThicknessLeft = "0";
        var borderThicknessRight = "0";

        var propertyEnumerator = localProperties.GetEnumerator();
        while (propertyEnumerator.MoveNext())
        {
            switch (propertyEnumerator.Key as string)
            {
                case "font-family":
                    //  Convert from font-family value list into xaml FontFamily value
                    xamlElement.SetAttribute(XamlFontFamily, (string?)propertyEnumerator.Value);
                    break;
                case "font-style":
                    xamlElement.SetAttribute(XamlFontStyle, (string?)propertyEnumerator.Value);
                    break;
                case "font-variant":
                    //  Convert from font-variant into xaml property
                    break;
                case "font-weight":
                    xamlElement.SetAttribute(XamlFontWeight, (string?)propertyEnumerator.Value);
                    break;
                case "font-size":
                    //  Convert from css size into FontSize
                    xamlElement.SetAttribute(XamlFontSize, (string?)propertyEnumerator.Value);
                    break;
                case "color":
                    SetPropertyValue(xamlElement, TextElement.ForegroundProperty, (string?)propertyEnumerator.Value);
                    break;
                case "background-color":
                    SetPropertyValue(xamlElement, TextElement.BackgroundProperty, (string?)propertyEnumerator.Value);
                    break;
                case "text-decoration-underline":
                    if (!isBlock && (string?)propertyEnumerator.Value == "true")
                    {
                        xamlElement.SetAttribute(XamlTextDecorations, XamlTextDecorationsUnderline);
                    }
                    break;
                case "text-decoration-none":
                case "text-decoration-overline":
                case "text-decoration-line-through":
                case "text-decoration-blink":
                    break;
                case "text-transform":
                    //  Convert from text-transform into xaml property
                    break;

                case "text-indent":
                    if (isBlock)
                    {
                        xamlElement.SetAttribute(XamlTextIndent, (string?)propertyEnumerator.Value);
                    }
                    break;

                case "text-align":
                    if (isBlock)
                    {
                        xamlElement.SetAttribute(XamlTextAlignment, (string?)propertyEnumerator.Value);
                    }
                    break;

                case "width":
                case "height":
                    //  Decide what to do with width and height propeties
                    break;

                case "margin-top":
                    marginSet = true;
                    marginTop = (string?)propertyEnumerator.Value;
                    break;
                case "margin-right":
                    marginSet = true;
                    marginRight = (string?)propertyEnumerator.Value;
                    break;
                case "margin-bottom":
                    marginSet = true;
                    marginBottom = (string?)propertyEnumerator.Value;
                    break;
                case "margin-left":
                    marginSet = true;
                    marginLeft = (string?)propertyEnumerator.Value;
                    break;

                case "padding-top":
                    paddingSet = true;
                    paddingTop = (string?)propertyEnumerator.Value;
                    break;
                case "padding-right":
                    paddingSet = true;
                    paddingRight = (string?)propertyEnumerator.Value;
                    break;
                case "padding-bottom":
                    paddingSet = true;
                    paddingBottom = (string?)propertyEnumerator.Value;
                    break;
                case "padding-left":
                    paddingSet = true;
                    paddingLeft = (string?)propertyEnumerator.Value;
                    break;

                // NOTE: css names for elementary border styles have side indications in the middle (top/bottom/left/right)
                // In our internal notation we intentionally put them at the end - to unify processing in ParseCssRectangleProperty method
                case "border-color-top":
                    borderColor = (string?)propertyEnumerator.Value;
                    break;
                case "border-color-right":
                    borderColor = (string?)propertyEnumerator.Value;
                    break;
                case "border-color-bottom":
                    borderColor = (string?)propertyEnumerator.Value;
                    break;
                case "border-color-left":
                    borderColor = (string?)propertyEnumerator.Value;
                    break;
                case "border-style-top":
                case "border-style-right":
                case "border-style-bottom":
                case "border-style-left":
                    //  Implement conversion from border style
                    break;
                case "border-width-top":
                    borderThicknessSet = true;
                    borderThicknessTop = (string?)propertyEnumerator.Value;
                    break;
                case "border-width-right":
                    borderThicknessSet = true;
                    borderThicknessRight = (string?)propertyEnumerator.Value;
                    break;
                case "border-width-bottom":
                    borderThicknessSet = true;
                    borderThicknessBottom = (string?)propertyEnumerator.Value;
                    break;
                case "border-width-left":
                    borderThicknessSet = true;
                    borderThicknessLeft = (string?)propertyEnumerator.Value;
                    break;

                case "list-style-type":
                    if (xamlElement.LocalName == XamlList)
                    {
                        var markerStyle = (((string?)propertyEnumerator.Value)?.ToLower(CultureInfo.InvariantCulture)) switch
                        {
                            "disc" => XamlListMarkerStyleDisc,
                            "circle" => XamlListMarkerStyleCircle,
                            "none" => XamlListMarkerStyleNone,
                            "square" => XamlListMarkerStyleSquare,
                            "box" => XamlListMarkerStyleBox,
                            "lower-latin" => XamlListMarkerStyleLowerLatin,
                            "upper-latin" => XamlListMarkerStyleUpperLatin,
                            "lower-roman" => XamlListMarkerStyleLowerRoman,
                            "upper-roman" => XamlListMarkerStyleUpperRoman,
                            "decimal" => XamlListMarkerStyleDecimal,
                            _ => XamlListMarkerStyleDisc,
                        };
                        xamlElement.SetAttribute(XamlListMarkerStyle, markerStyle);
                    }
                    break;

                case "float":
                case "clear":
                    if (isBlock)
                    {
                        //  Convert float and clear properties
                    }
                    break;

                case "display":
                    break;
                default:
                    break;
            }
        }

        if (isBlock)
        {
            if (marginSet)
            {
                ComposeThicknessProperty(xamlElement, XamlMargin, marginLeft, marginRight, marginTop, marginBottom);
            }

            if (paddingSet)
            {
                ComposeThicknessProperty(xamlElement, XamlPadding, paddingLeft, paddingRight, paddingTop, paddingBottom);
            }

            if (borderColor != null)
            {
                //  We currently ignore possible difference in brush colors on different border sides. Use the last colored side mentioned
                xamlElement.SetAttribute(XamlBorderBrush, borderColor);
            }

            if (borderThicknessSet)
            {
                ComposeThicknessProperty(xamlElement, XamlBorderThickness, borderThicknessLeft, borderThicknessRight, borderThicknessTop, borderThicknessBottom);
            }
        }
    }

    // Create syntactically optimized four-value Thickness
    private static void ComposeThicknessProperty(XmlElement xamlElement, string propertyName, string? left, string? right, string? top, string? bottom)
    {
        // Xaml syntax:
        // We have a reasonable interpreation for one value (all four edges), two values (horizontal, vertical),
        // and four values (left, top, right, bottom).
        string? thickness;

        // We do not accept negative margins
        if (left?[0] is '0' or '-') left = "0";
        if (right?[0] is '0' or '-') right = "0";
        if (top?[0] is '0' or '-') top = "0";
        if (bottom?[0] is '0' or '-') bottom = "0";

        thickness = left == right && top == bottom ? left == top ? left : left + "," + top : left + "," + top + "," + right + "," + bottom;

        //  Need safer processing for a thickness value
        xamlElement.SetAttribute(propertyName, thickness);
    }

    private static void SetPropertyValue(XmlElement xamlElement, DependencyProperty property, string? stringValue)
    {
        var typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(property.PropertyType);
        try
        {
            var convertedValue = typeConverter.ConvertFromInvariantString(stringValue ?? string.Empty);
            if (convertedValue != null)
            {
                xamlElement.SetAttribute(property.Name, stringValue);
            }
        }
        catch (Exception)
        {
            // Nothing
        }
    }

    /// <summary>
    /// Analyzes the tag of the htmlElement and infers its associated formatted properties.
    /// After that parses style attribute and adds all inline css styles.
    /// The resulting style attributes are collected in output parameter localProperties.
    /// </summary>
    /// <param name="htmlElement">
    /// </param>
    /// <param name="inheritedProperties">
    /// set of properties inherited from ancestor elements. Currently not used in the code. Reserved for the future development.
    /// </param>
    /// <param name="localProperties">
    /// returns all formatting properties defined by this element - implied by its tag, its attributes, or its css inline style
    /// </param>
    /// <param name="stylesheet"></param>
    /// <param name="sourceContext"></param>
    /// <returns>
    /// returns a combination of previous context with local set of properties.
    /// This value is not used in the current code - inntended for the future development.
    /// </returns>
    private static Hashtable GetElementProperties(XmlElement htmlElement, Hashtable inheritedProperties, out Hashtable localProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
    {
        // Start with context formatting properties
        var currentProperties = new Hashtable();
        var propertyEnumerator = inheritedProperties.GetEnumerator();
        while (propertyEnumerator.MoveNext())
        {
            currentProperties[propertyEnumerator.Key] = propertyEnumerator.Value;
        }

        // Identify element name
        var elementName = htmlElement.LocalName.ToLower(CultureInfo.InvariantCulture);
        _ = htmlElement.NamespaceURI;

        // update current formatting properties depending on element tag

        localProperties = [];
        switch (elementName)
        {
            // Character formatting
            case "i":
            case "italic":
            case "em":
                localProperties["font-style"] = "italic";
                break;
            case "b":
            case "bold":
            case "strong":
            case "dfn":
                localProperties["font-weight"] = "bold";
                break;
            case "u":
            case "underline":
                localProperties["text-decoration-underline"] = "true";
                break;
            case "font":
                var attributeValue = GetAttribute(htmlElement, "face");
                if (attributeValue != null)
                {
                    localProperties["font-family"] = attributeValue;
                }
                attributeValue = GetAttribute(htmlElement, "size");
                if (attributeValue != null)
                {
                    var fontSize = double.Parse(attributeValue, CultureInfo.InvariantCulture) * (12.0 / 3.0);
                    if (fontSize < 1.0)
                    {
                        fontSize = 1.0;
                    }
                    else if (fontSize > 1000.0)
                    {
                        fontSize = 1000.0;
                    }
                    localProperties["font-size"] = fontSize.ToString(CultureInfo.InvariantCulture);
                }
                attributeValue = GetAttribute(htmlElement, "color");
                if (attributeValue != null)
                {
                    localProperties["color"] = attributeValue;
                }
                break;
            case "pre":
            case "samp":
                localProperties["font-family"] = "Courier New"; // code sample
                localProperties["font-size"] = XamlFontSizeXXSmall;
                localProperties["text-align"] = "Left";
                break;
            case "sub":
                break;
            case "sup":
                break;

            // Hyperlinks
            case "a": // href, hreflang, urn, methods, rel, rev, title
                //  Set default hyperlink properties
                break;
            case "acronym":
                break;

            // Paragraph formatting:
            case "p":
                //  Set default paragraph properties
                break;
            case "div":
                //  Set default div properties
                break;
            case "blockquote":
                localProperties["margin-left"] = "16";
                break;

            case "h1":
                localProperties["font-size"] = XamlFontSizeXXLarge;
                break;
            case "h2":
                localProperties["font-size"] = XamlFontSizeXLarge;
                break;
            case "h3":
                localProperties["font-size"] = XamlFontSizeLarge;
                break;
            case "h4":
                localProperties["font-size"] = XamlFontSizeMedium;
                break;
            case "h5":
                localProperties["font-size"] = XamlFontSizeSmall;
                break;
            case "h6":
                localProperties["font-size"] = XamlFontSizeXSmall;
                break;
            // List properties
            case "ul":
                localProperties["list-style-type"] = "disc";
                break;
            case "ol":
                localProperties["list-style-type"] = "decimal";
                break;

            case "table":
            case "body":
            case "html":
                break;
            default:
                break;
        }

        // Override html defaults by css attributes - from stylesheets and inline settings
        HtmlCssParser.GetElementPropertiesFromCssAttributes(htmlElement, elementName, stylesheet, localProperties, sourceContext);

        // Combine local properties with context to create new current properties
        propertyEnumerator = localProperties.GetEnumerator();
        while (propertyEnumerator.MoveNext())
        {
            currentProperties[propertyEnumerator.Key] = propertyEnumerator.Value;
        }

        return currentProperties;
    }

    /// <summary>
    /// Extracts a value of css attribute from css style definition.
    /// </summary>
    /// <param name="cssStyle">
    /// Source csll style definition
    /// </param>
    /// <param name="attributeName">
    /// A name of css attribute to extract
    /// </param>
    /// <returns>
    /// A string rrepresentation of an attribute value if found;
    /// null if there is no such attribute in a given string.
    /// </returns>
    private static string? GetCssAttribute(string? cssStyle, string attributeName)
    {
        //  This is poor man's attribute parsing. Replace it by real css parsing
        if (cssStyle != null)
        {
            string[] styleValues;

            attributeName = attributeName.ToLower(CultureInfo.InvariantCulture);

            // Check for width specification in style string
            styleValues = cssStyle.Split(';');

            for (var styleValueIndex = 0; styleValueIndex < styleValues.Length; styleValueIndex++)
            {
                string[] styleNameValue;

                styleNameValue = styleValues[styleValueIndex].Split(':');
                if (styleNameValue.Length == 2 && styleNameValue[0].Trim().Equals(attributeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return styleNameValue[1].Trim();
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Converts a length value from string representation to a double.
    /// </summary>
    /// <param name="lengthAsString">
    /// Source string value of a length.
    /// </param>
    /// <param name="length"></param>
    /// <returns></returns>
    private static bool TryGetLengthValue(string? lengthAsString, out double length)
    {
        length = double.NaN;

        if (lengthAsString != null)
        {
            lengthAsString = lengthAsString.Trim().ToLower(CultureInfo.InvariantCulture);

            // We try to convert currentColumnWidthAsString into a double. This will eliminate widths of type "50%", etc.
            if (lengthAsString.EndsWith("pt", StringComparison.OrdinalIgnoreCase))
            {
                lengthAsString = lengthAsString[0..^2];
                if (double.TryParse(lengthAsString, out length))
                {
                    length = length * 96.0 / 72.0; // convert from points to pixels
                }
                else
                {
                    length = double.NaN;
                }
            }
            else if (lengthAsString.EndsWith("px", StringComparison.OrdinalIgnoreCase))
            {
                lengthAsString = lengthAsString[0..^2];
                if (!double.TryParse(lengthAsString, out length))
                {
                    length = double.NaN;
                }
            }
            else
            {
                if (!double.TryParse(lengthAsString, out length)) // Assuming pixels
                {
                    length = double.NaN;
                }
            }
        }

        return !double.IsNaN(length);
    }

    // .................................................................
    //
    // Pasring Color Attribute
    //
    // .................................................................

    /// <summary>
    /// Applies properties to xamlTableCellElement based on the html td element it is converted from.
    /// </summary>
    /// <param name="htmlChildNode">
    /// Html td/th element to be converted to xaml
    /// </param>
    /// <param name="xamlTableCellElement">
    /// XmlElement representing Xaml element for which properties are to be processed
    /// </param>
    private static void ApplyPropertiesToTableCellElement(XmlElement htmlChildNode, XmlElement xamlTableCellElement)
    {
        // Parameter validation
        Debug.Assert(htmlChildNode.LocalName.ToLower(CultureInfo.InvariantCulture) is "td" or "th");
        Debug.Assert(xamlTableCellElement.LocalName == XamlTableCell);

        // set default border thickness for xamlTableCellElement to enable gridlines
        xamlTableCellElement.SetAttribute(XamlTableCellBorderThickness, "1,1,1,1");
        xamlTableCellElement.SetAttribute(XamlTableCellBorderBrush, XamlBrushesBlack);
        var rowSpanString = GetAttribute(htmlChildNode, "rowspan");
        if (rowSpanString != null)
        {
            xamlTableCellElement.SetAttribute(XamlTableCellRowSpan, rowSpanString);
        }
    }

    #endregion Private Methods

    // ----------------------------------------------------------------
    //
    // Internal Constants
    //
    // ----------------------------------------------------------------

    // The constants reprtesent all Xaml names used in a conversion
    /// <summary></summary>
    public const string XamlFlowDocument = "FlowDocument";
    /// <summary></summary>
    public const string XamlRun = "Run";
    /// <summary></summary>
    public const string XamlSpan = "Span";
    /// <summary></summary>
    public const string XamlHyperlink = "Hyperlink";
    /// <summary></summary>
    public const string XamlHyperlinkNavigateUri = "NavigateUri";
    /// <summary></summary>
    public const string XamlHyperlinkTargetName = "TargetName";
    /// <summary></summary>
    public const string XamlSection = "Section";
    /// <summary></summary>
    public const string XamlList = "List";
    /// <summary></summary>
    public const string XamlListMarkerStyle = "MarkerStyle";
    /// <summary></summary>
    public const string XamlListMarkerStyleNone = "None";
    /// <summary></summary>
    public const string XamlListMarkerStyleDecimal = "Decimal";
    /// <summary></summary>
    public const string XamlListMarkerStyleDisc = "Disc";
    /// <summary></summary>
    public const string XamlListMarkerStyleCircle = "Circle";
    /// <summary></summary>
    public const string XamlListMarkerStyleSquare = "Square";
    /// <summary></summary>
    public const string XamlListMarkerStyleBox = "Box";
    /// <summary></summary>
    public const string XamlListMarkerStyleLowerLatin = "LowerLatin";
    /// <summary></summary>
    public const string XamlListMarkerStyleUpperLatin = "UpperLatin";
    /// <summary></summary>
    public const string XamlListMarkerStyleLowerRoman = "LowerRoman";
    /// <summary></summary>
    public const string XamlListMarkerStyleUpperRoman = "UpperRoman";
    /// <summary></summary>
    public const string XamlListItem = "ListItem";
    /// <summary></summary>
    public const string XamlLineBreak = "LineBreak";
    /// <summary></summary>
    public const string XamlParagraph = "Paragraph";
    /// <summary></summary>
    public const string XamlMargin = "Margin";
    /// <summary></summary>
    public const string XamlPadding = "Padding";
    /// <summary></summary>
    public const string XamlBorderBrush = "BorderBrush";
    /// <summary></summary>
    public const string XamlBorderThickness = "BorderThickness";
    /// <summary></summary>
    public const string XamlTable = "Table";
    /// <summary></summary>
    public const string XamlTableColumn = "TableColumn";
    /// <summary></summary>
    public const string XamlTableRowGroup = "TableRowGroup";
    /// <summary></summary>
    public const string XamlTableRow = "TableRow";
    /// <summary></summary>
    public const string XamlTableCell = "TableCell";
    /// <summary></summary>
    public const string XamlTableCellBorderThickness = "BorderThickness";
    /// <summary></summary>
    public const string XamlTableCellBorderBrush = "BorderBrush";
    /// <summary></summary>
    public const string XamlTableCellColumnSpan = "ColumnSpan";
    /// <summary></summary>
    public const string XamlTableCellRowSpan = "RowSpan";
    /// <summary></summary>
    public const string XamlWidth = "Width";
    /// <summary></summary>
    public const string XamlBrushesBlack = "Black";
    /// <summary></summary>
    public const string XamlFontFamily = "FontFamily";
    /// <summary></summary>
    public const string XamlFontSize = "FontSize";
    /// <summary></summary>
    public const string XamlFontSizeXXLarge = "22pt";
    /// <summary></summary>
    public const string XamlFontSizeXLarge = "20pt";
    /// <summary></summary>
    public const string XamlFontSizeLarge = "18pt";
    /// <summary></summary>
    public const string XamlFontSizeMedium = "16pt";
    /// <summary></summary>
    public const string XamlFontSizeSmall = "12pt";
    /// <summary></summary>
    public const string XamlFontSizeXSmall = "10pt";
    /// <summary></summary>
    public const string XamlFontSizeXXSmall = "8pt";
    /// <summary></summary>
    public const string XamlFontWeight = "FontWeight";
    /// <summary></summary>
    public const string XamlFontWeightBold = "Bold";
    /// <summary></summary>
    public const string XamlFontStyle = "FontStyle";
    /// <summary></summary>
    public const string XamlForeground = "Foreground";
    /// <summary></summary>
    public const string XamlBackground = "Background";
    /// <summary></summary>
    public const string XamlTextDecorations = "TextDecorations";
    /// <summary></summary>
    public const string XamlTextDecorationsUnderline = "Underline";
    /// <summary></summary>
    public const string XamlTextIndent = "TextIndent";
    /// <summary></summary>
    public const string XamlTextAlignment = "TextAlignment";

    // ---------------------------------------------------------------------
    //
    // Private Fields
    //
    // ---------------------------------------------------------------------

    #region Private Fields

    private static readonly string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

    #endregion Private Fields
}
