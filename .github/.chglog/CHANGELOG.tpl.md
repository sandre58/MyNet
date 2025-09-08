# Changelog for {{ .Info.Title }}

**Repository:** [{{ .Info.RepositoryURL }}]({{ .Info.RepositoryURL }})

{{ if .Versions -}}
{{ if .Unreleased.CommitGroups -}}
## [Unreleased]
{{ range .Unreleased.CommitGroups -}}
### {{ .Title }}
{{ range .Commits -}}
- {{ if .Scope }}**{{ .Scope }}:** {{ end }}{{ .Subject }}
{{ end }}
{{ end }}
{{ end }}

{{ range .Versions }}
{{ if .Tag.Previous -}}
## [{{ .Tag.Name }}] - {{ .Tag.Date | date "2006-01-02" }}
{{ else -}}
## {{ .Tag.Name }} - {{ .Tag.Date | date "2006-01-02" }}
{{ end -}}

{{ range .CommitGroups -}}
### {{ .Title }}
{{ range .Commits -}}
- {{ if .Scope }}**{{ .Scope }}:** {{ end }}{{ .Subject }}{{ if .Hash }} ([{{ .Hash.Short }}]({{ $.Info.RepositoryURL }}/commit/{{ .Hash.Long }})){{ end }}
{{ end }}
{{ end }}
{{ if .RevertCommits -}}

### 🔄 Reverts
{{ range .RevertCommits -}}
- {{ .Revert.Header }}
{{ end }}
{{ end }}

{{ if .MergeCommits -}}
### 🔀 Merges
{{ range .MergeCommits -}}
- {{ .Header }}
{{ end }}
{{ end }}

{{ if .NoteGroups -}}# Changelog for {{ .Info.Title }}

All notable changes to this project will be documented in this file.  
This file is generated automatically based on commit history and tags.

{{ if .Unreleased.CommitGroups -}}
## [Unreleased]
{{ range .Unreleased.CommitGroups -}}
### {{ .Title }}
{{ range .Commits -}}
- {{ if .Scope }}**{{ .Scope }}:** {{ end }}{{ .Subject }}
{{ end }}
{{ end }}
{{ end }}
{{ end }}

{{ range .Versions }}
## [{{ replace .Tag.Name (printf "%s/v" $.Info.Title) "" -1 }}] - {{ .Tag.Date | date "2006-01-02" }}
{{ range .CommitGroups -}}
### {{ .Title }}
{{ range .Commits -}}
- {{ if .Scope }}**{{ .Scope }}:** {{ end }}{{ .Subject }}{{ if .Hash }} ([{{ .Hash.Short }}]({{ $.Info.RepositoryURL }}/commit/{{ .Hash.Long }})){{ end }}
{{ end }}
{{ end }}

{{ if .RevertCommits -}}
### 🔄 Reverts
{{ range .RevertCommits -}}
- {{ .Revert.Header }}
{{ end }}
{{ end }}

{{ if .MergeCommits -}}
### 🔀 Merges
{{ range .MergeCommits -}}
- {{ .Header }}
{{ end }}
{{ end }}

{{ if .NoteGroups -}}
{{ range .NoteGroups -}}
### ⚠️ {{ .Title }}
{{ range .Notes }}
- {{ .Body }}
{{ end }}
{{ end }}
{{ end }}

{{ if .Tag.Previous }}
**Full Changelog:** [compare {{ .Tag.Previous.Name }}...{{ .Tag.Name }}]({{ $.Info.RepositoryURL }}/compare/{{ .Tag.Previous.Name }}...{{ .Tag.Name }})
{{ end }}

---
{{ end }}
{{ range .NoteGroups -}}
### ⚠️ {{ .Title }}
{{ range .Notes }}
{{ .Body }}
{{ end }}
{{ end }}
{{ end }}

{{ end }}
{{ else }}
## 📝 Changes
{{ range .Commits -}}
- {{ if .Scope }}**{{ .Scope }}:** {{ end }}{{ .Subject }}{{ if .Hash }} ([{{ .Hash.Short }}]({{ .Info.RepositoryURL }}/commit/{{ .Hash.Long }})){{ end }}
{{ end }}
{{ end }}
