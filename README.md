![WhatsApp Image 2025-09-27 at 10 19 25 AM](https://github.com/user-attachments/assets/af001b95-e0aa-4480-8b75-ca6cad432e6b)
![WhatsApp Image 2025-09-13 at 5 40 03 PM](https://github.com/user-attachments/assets/d390fc8a-5148-4602-8cef-81d3437f3c97)

# 🎓 Sistema de Notas Académicas

**Autores:**  
- Emerson Jesús Londoño Buitrago  
- Emmanuel Álvarez Franco  
- Sebastián Gómez López  

**Presentado a:** Willian Díaz Villegas  
**Asignatura:** Aplicación y Servicios Web  
**Institución:** Instituto Tecnológico Metropolitano (ITM)  
**Ciudad:** Medellín  
**Año:** 2025  

---

## 🧠 Descripción General

El **Sistema de Notas Académicas** es una aplicación web desarrollada con **ASP.NET Core MVC** y **API REST** que optimiza la gestión de calificaciones en instituciones universitarias.  
Resuelve problemas comunes como el registro manual de notas, los errores en el cálculo de promedios y la falta de reportes claros y automatizados.  

El sistema permite registrar estudiantes, asignaturas y programas académicos; gestionar cursos, planes de evaluación y calificaciones; calcular automáticamente los promedios y generar reportes académicos en PDF desde una plataforma centralizada y segura.

---

## 🎯 Objetivos Específicos 
Fase del Ciclo de Desarrollo	Objetivo Específico
1. Análisis	Identificar los requerimientos funcionales y no funcionales del sistema de notas académicas mediante la especificación de casos de uso y entidades del dominio.
2. Diseño	Diseñar la arquitectura en capas (MVC) que permita el desacoplamiento entre la lógica de negocio, la capa de datos y la interfaz de usuario, incluyendo la definición de interfaces y DTOs.
3. Implementación	Implementar el caso de uso de gestión de calificaciones utilizando Entity Framework Core como ORM para mapear correctamente las entidades y relaciones entre tablas.
4. Integración	Integrar los controladores MVC y la API REST para exponer los endpoints de negocio, documentando los servicios con Swagger / OpenAPI.
5. Pruebas y Validación	Validar la persistencia de datos, el funcionamiento de los cálculos de promedios y la correcta respuesta de los endpoints mediante pruebas funcionales y de integración.
6. Despliegue y Mantenimiento	Configurar la base de datos en SQL Server Express, habilitar migraciones automáticas y garantizar la portabilidad del sistema mediante contenedores o scripts estándar.

---

## ⚙️ Requisitos Funcionales

| Código | Descripción |
|:------:|--------------|
| RF-01 | Autenticación y autorización (roles: Admin, Docente, Estudiante). |
| RF-02 | Gestión de catálogos: programas, asignaturas, periodos, cursos. |
| RF-03 | Administración de usuarios y roles. |
| RF-04 | Creación de planes de evaluación por curso (porcentaje, fechas). |
| RF-05 | Matrícula de estudiantes en cursos. |
| RF-06 | Registro de calificaciones (unitario o masivo). |
| RF-07 | Cálculo automático de promedios y notas finales. |
| RF-08 | Cierre de actas y generación de reportes en PDF. |
| RF-09 | Reportes por curso, estudiante y programa. |
| RF-10 | Historial de calificaciones y trazabilidad. |
| RF-11 | Notificaciones de actualización y cierre de notas. |
| RF-12 | Búsquedas y filtros avanzados. |

---

## 🧩 Requisitos No Funcionales

| Código | Descripción |
|:------:|--------------|
| RNF-01 | Seguridad: TLS 1.2+, contraseñas cifradas, control RBAC, protección CSRF/XSS/SQLi. |
| RNF-02 | Rendimiento: consultas p95 < 2s, reportes PDF ≤ 4s (hasta 100 estudiantes). |
| RNF-03 | Disponibilidad: 99% en periodos de evaluación. |
| RNF-04 | Usabilidad: interfaz responsive, accesibilidad AA (WCAG 2.1). |
| RNF-05 | Mantenibilidad: separación en capas (MVC), pruebas unitarias ≥70%. |
| RNF-06 | Auditabilidad: logs estructurados, trazabilidad de notas. |
| RNF-07 | Portabilidad: despliegue mediante contenedor o SQL estándar. |

---

## 🏗️ Arquitectura del Sistema

**Patrón:** MVC (Model–View–Controller) + API REST  
**ORM:** Entity Framework Core  
**Base de Datos:** Microsoft SQL Server Express (Autenticación Windows)  
**Documentación de API:** Swagger / OpenAPI  
**Lenguaje:** C# (.NET 9.0.9)  

```plaintext
┌────────────────────────┐
│        CLIENTE         │
│ (Navegador Web/Swagger)│
└────────────┬───────────┘
             │
             ▼
┌────────────────────────┐
│  ASP.NET Core MVC + API│
│ Controllers / Views /   │
│ REST Endpoints          │
└────────────┬───────────┘
             │
             ▼
┌────────────────────────┐
│   Entity Framework Core │
│   (LINQ / Migrations)   │
└────────────┬───────────┘
             │
             ▼
┌────────────────────────┐
│  SQL Server Express DB  │
└────────────────────────┘

