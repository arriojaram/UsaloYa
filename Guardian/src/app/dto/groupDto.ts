export interface GroupDto {
  groupId?: number;          // Opcional en creación
  name: string;
  description: string;
  permissions?: string;      // Opcional si no se usa al registrar
  companyId?: number;        // Se puede omitir en el frontend si lo asigna el backend
}
