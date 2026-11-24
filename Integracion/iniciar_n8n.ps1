# Importa, publica y arranca el workflow "Planta de Reciclaje ODS 12" en n8n local (http://localhost:5678).
$root = Split-Path $PSScriptRoot -Parent
$env:RECICLAJE_CSV = Join-Path $root 'Data\residuos_solidos_sjm_2026.csv'
$env:N8N_RESTRICT_FILE_ACCESS_TO = Join-Path $root 'Data'
$env:N8N_BLOCK_ENV_ACCESS_IN_NODE = 'false'
$env:N8N_SECURE_COOKIE = 'false'

n8n import:workflow --input="$PSScriptRoot\n8n\planta_reciclaje_workflow.json"
n8n publish:workflow --id=PlantaReciclajeODS12
n8n start
