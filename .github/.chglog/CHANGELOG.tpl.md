# Changelog for {{ .Info.Title }} v{{ replace (index .Versions 0).Tag.Name (printf "%s/v" .Info.Title) "" -1 }}

**Released:** {{ (index .Versions 0).Tag.Date | date "2006-01-02 15:04:05 UTC" }}  
**Tag:** {{ (index .Versions 0).Tag.Name }}  
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
## [{{ replace .Tag.Name (printf "%s/v" $.Info.Title) "" -1 }}] - {{ .Tag.Date | date "2006-01-02" }}
{{ else -}}
## [{{ replace .Tag.Name (printf "%s/v" $.Info.Title) "" -1 }}] - {{ .Tag.Date | date "2006-01-02" }}
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

{{ if .NoteGroups -}}
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

---

## 📦 Package Information

- **Project:** {{ .Info.Title }}
- **Version:** {{ replace .Tag.Name (printf "%s/v" .Info.Title) "" -1 }}
- **Commit:** `{{ .Tag.Hash.Long }}`

{{ if .NoteGroups -}}
{{ range .NoteGroups -}}
{{ if eq .Title "BREAKING CHANGES" -}}
---

## ⚠️ Breaking Changes

{{ range .Notes }}
{{ .Body }}
{{ end }}
{{ end }}
{{ end }}
{{ end }}
