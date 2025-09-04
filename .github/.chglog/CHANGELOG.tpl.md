# Changelog for {{ .Info.Title }} v{{ .Tag.Name | replace (printf "%s/v" .Info.Title) "" }}

**Released:** {{ .Tag.Date | date "2006-01-02 15:04:05 UTC" }}  
**Tag:** {{ .Tag.Name }}  
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
## [{{ .Tag.Name | replace (printf "%s/v" $.Info.Title) "" }}] - {{ .Tag.Date | date "2006-01-02" }}
{{ else -}}
## {{ .Tag.Name | replace (printf "%s/v" $.Info.Title) "" }} - {{ .Tag.Date | date "2006-01-02" }}
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
- **Version:** {{ .Tag.Name | replace (printf "%s/v" .Info.Title) "" }}
- **Commit:** `{{ .Tag.Hash.Long }}`
- **Build:** [GitHub Actions Run #${{ github.run_number }}](${{ github.server_url }}/${{ github.repository }}/actions/runs/${{ github.run_id }})

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
