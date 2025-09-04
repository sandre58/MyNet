# {{ .Info.Title }} v{{ .Tag.Name | replace (printf "%s/v" .Info.Title) "" }}

{{ if .Versions -}}
{{ range .Versions }}
{{ range .CommitGroups -}}
## {{ .Title }}
{{ range .Commits -}}
- {{ if .Scope }}**{{ .Scope }}:** {{ end }}{{ .Subject }}{{ if .Hash }} ([{{ .Hash.Short }}]({{ $.Info.RepositoryURL }}/commit/{{ .Hash.Long }})){{ end }}
{{ end }}
{{ end }}

{{ if .RevertCommits -}}
## 🔄 Reverts
{{ range .RevertCommits -}}
- {{ .Revert.Header }}
{{ end }}
{{ end }}

{{ if .NoteGroups -}}
{{ range .NoteGroups -}}
## ⚠️ {{ .Title }}
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

---

**Full Changelog:** [{{ .Info.Title }}/CHANGELOG.md]({{ .Info.RepositoryURL }}/blob/main/{{ .Info.Title | replace "." "/" | lower }}/CHANGELOG.md)
