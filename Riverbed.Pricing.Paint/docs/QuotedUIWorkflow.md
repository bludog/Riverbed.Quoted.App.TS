```mermaid

    flowchart LR
        A[Start] --> A1[Menu]
        A --> A2[Company]
        A --> A3[Administration]
        A --> A4[Acount]

        A1 --> B[Customers]
        A1 --> C[Projects]
        C --> M[Projects By Date]
        M --> C5

        B --> C1[Project List]
        C1 --> C2[Create New Project]
        C1 --> C3[Edit Customer]
        C1 --> C4[Delete Customer]
        C1 --> C5[View Project]

        C5 --> C1[Project List]
        C5 --> C6[Edit Project]
        C5 --> C7[Delete Project]
        C5 --> C8[Project Global Mods]

        C5 --> L1[Project Options]
        L1 --> M1[Change Project Status]
        L1 --> M2[Emails]
        L1 --> M3[Create Estimate]
        L1 --> M4[Estimate Report List]
        L1 --> M5[Email History]

        M2 --> N1[Send Email]

        C5 --> C9[Room List]          
        C9 --> C10[Edit Room]
        C10 --> C9
        C9 --> C11[Add House]
        C11 --> C15[House Wizard]
        C15 --> C9
        C9 --> C12[Add Room]
        C12 --> C14[Room Wizard]
        C14 --> C10
         
        A2 --> D[Company Settings]
        A2 --> E[Report Editor]

        A3 --> F[Global Settings]
        A3 --> G[Company Admin]
        A3 --> H[Projects By Company]
        A3 --> I[Download PDF]
        A3 --> J[Upload Files]
        A3 --> K[Site Map]

        A4 --> L[User Profile]
        
        

