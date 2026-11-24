# Datos abiertos – ODS 12: Planta de reciclaje

## residuos_solidos_sjm_2026.csv

| Campo | Valor |
|---|---|
| Dataset | Generación y reaprovechamiento de residuos sólidos del distrito de San Juan de Miraflores |
| Publicador | Municipalidad Distrital de San Juan de Miraflores (RUC 20131378204) |
| Portal | Plataforma Nacional de Datos Abiertos del Perú |
| URL del dataset | https://www.datosabiertos.gob.pe/dataset/generaci%C3%B3n-y-reaprovechamiento-de-residuos-s%C3%B3lidos-del-distrito-de-san-juan-de-miraflores |
| URL del archivo | https://www.datosabiertos.gob.pe/sites/default/files/residuos_solidos_sjm_2026.csv |
| Fecha de descarga | 2026-11-24 |
| Periodo | Enero – abril 2026 (mensual) |
| Licencia | Open Data Commons Attribution License (ODC-BY) – https://opendatacommons.org/licenses/by/ |
| Formato | CSV separado por `;`, UTF-8 con BOM |

Datos agregados (toneladas por mes), sin datos personales y sin necesidad de permisos ni credenciales.

### Columnas

- `VC_RUC_ENTIDAD`: RUC de la municipalidad.
- `IN_ANNO`: año.
- `VC_MES`: mes (la última fila `TOTAL` es el acumulado).
- `VC_SERVICIO_RECOLECCION`: tipo de servicio de recolección.
- `VC_UNIDAD_MEDIDA`: unidad (tonelada).
- `MO_RESIDUOS_NO_APROVECHABLES`: toneladas de residuos no aprovechables.
- `MO_RESIDUOS_APROVECHABLES`: toneladas de residuos aprovechables (reciclables).

### Uso en el proyecto

- El flujo n8n (`Integracion/`) lee este CSV y calcula la tasa de reaprovechamiento del distrito. La muestra en el dashboard junto a los eventos de clasificación que llegan desde la simulación VR.
- Una copia está en `Assets/StreamingAssets/` de ambos proyectos Unity. Así el panel informativo de la planta puede mostrar los datos aunque n8n no esté disponible.
