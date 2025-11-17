![WhatsApp Image 2025-09-27 at 10 19 25 AM](https://github.com/user-attachments/assets/af001b95-e0aa-4480-8b75-ca6cad432e6b)
![WhatsApp Image 2025-09-13 at 5 40 03 PM](https://github.com/user-attachments/assets/d390fc8a-5148-4602-8cef-81d3437f3c97)

Diagrama de clases (Actualizado )

<img width="920" height="557" alt="image" src="https://github.com/user-attachments/assets/cd142456-b196-459a-bab2-bd02b4b2cf05" />

Diagrama UML por capas 

<img width="920" height="618" alt="image" src="https://github.com/user-attachments/assets/b698c353-1dea-4887-98dc-40bd092b815e" />


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


El Sistema de Gestión de Calificaciones Académicas es una plataforma digital diseñada para automatizar el proceso académico relacionado con el registro, cálculo, administración y generación de reportes de notas dentro de una institución educativa. El sistema integra módulos para gestionar usuarios, cursos, programas académicos, planes de evaluación y calificaciones, garantizando precisión, seguridad y eficiencia.

El proyecto utiliza una arquitectura basada en MVC, apoyada en una API REST para la comunicación entre capas y una base de datos relacional implementada en SQL mediante Entity Framework Core. El objetivo central es proporcionar una herramienta moderna, organizada y confiable que optimice la gestión académica tanto para docentes como para estudiantes.


---

## 📓 Objetivo del Proyecto
Desarrollar un sistema informático que permita gestionar de manera automatizada las calificaciones académicas de los estudiantes, integrando el registro de datos, el cálculo de promedios y la generación de reportes académicos.

---

## 🎯 Objetivos Específicos 
1.	Realizar el análisis de los requisitos funcionales y no funcionales del sistema de gestión de calificaciones.
2.	Diseñar la estructura del sistema, incluyendo la arquitectura por capas, los modelos de datos y los diagramas UML.
3.	Implementar los módulos del sistema correspondientes al registro de estudiantes, asignaturas y calificaciones.
4.	Probar el funcionamiento del sistema para garantizar la confiabilidad del cálculo de promedios y la generación de reportes.
5.	Implantar el sistema desarrollado para su uso como herramienta de gestión académica institucional.
   
---

## 🗺️ Esquema del proyecto de software: 

<img width="920" height="625" alt="image" src="https://github.com/user-attachments/assets/23a81fd2-894b-4d5b-afc9-d6358c8d7605" />

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

