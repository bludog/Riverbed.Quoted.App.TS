# Development Prompt: Replicating a Dynamic Project Management Workspace

## 1. Objective

Develop a single-page React component that replicates the core functionality and user interface of the provided Monday.com-style project management workspace. The component must be a high-fidelity, interactive, and performant replica of the visual and functional elements depicted in the user-provided image. The most critical feature is the implementation of seamless drag-and-drop functionality for organizing project items between different status groups.

## 2. Core Functionality

### 2.1. Hierarchical Structure

The application should follow a three-tiered data structure:

- **Board**: The main container for the entire workspace. It has a title (e.g., "Customer Projects") and holds multiple groups.
- **Groups**: These are collapsible sections that categorize items, such as "Up And Coming", "In-Progress Projects", and "Completed Waiting On Payment". Each group must have a distinct color and a header that displays its title and a collapse/expand toggle.
- **Items**: These are the individual rows within a group, representing a project or task. Each item contains data across multiple columns.

### 2.2. Seamless Drag-and-Drop

This is a mandatory and critical feature. The implementation must be smooth, intuitive, and provide clear visual feedback.

- **Item Reordering (Intra-group)**: Users must be able to click and drag an item to change its position within the same group.
- **Item Moving (Inter-group)**: Users must be able to drag an item from one group and drop it into another. This action signifies a change in the item's status (e.g., moving a project from "Up And Coming" to "In-Progress Projects").
- **Group Reordering**: Users must be able to drag and drop an entire group to reorder its position on the board.
- **Visual Feedback**: During a drag operation, the dragged item should be visually distinct (e.g., semi-transparent or with a box shadow). The potential drop location (drop zone) should be clearly highlighted to indicate where the item will be placed upon release.

### 2.3. Column Types and Data Representation

The component must support various column types as seen in the image, each with specific rendering and interaction logic.

| Column Name      | Type           | Description                                                                                                  |
| ---------------- | -------------- | ------------------------------------------------------------------------------------------------------------ |
| **Project**      | Text           | The primary identifier for the item.                                                                         |
| **Timeline**     | Date Range     | Displays a start and end date with a colored visual bar representing the duration. The bar's color should match the group's color. |
| **Status**       | Categorical    | Displays a colored badge with text (e.g., "Done", "Ready", "Paid"). The color should be configurable based on the status. |
| **Currency**     | Number         | Displays numerical values formatted as currency (e.g., $1,800.00). Must be right-aligned.                     |
| **Selection**    | Checkbox       | A checkbox on the far left of each row to enable multi-item selection and batch actions.                     |

### 2.4. Group Summaries

At the bottom of each group, a summary row must be displayed. This row should automatically calculate and show the aggregate totals for all numerical columns (e.g., 'Total Job', 'Labor', 'Materials') for the items within that group. This summary must update in real-time when items are added, removed, or moved between groups.

## 3. UI/UX Design Specifications

### 3.1. Layout and Theme

- **Theme**: Implement a dark theme with a dark navy/purple background (approx. `#2B2D3E`).
- **Layout**: A full-width, multi-column table layout. The board should support horizontal scrolling if the columns exceed the viewport width.
- **Header**: A fixed header containing the board title, view tabs, and primary action buttons ("New project", "Search", "Filter", etc.).

### 3.2. Color Palette

- **Background**: Dark Navy/Purple (e.g., `#2B2D3E`)
- **Text**: White / Light Gray
- **Group Headers**: Customizable colors (Orange-Red, Blue, Yellow-Gold as seen in the image).
- **Status Badges**: Green for "Done"/"Ready", Light Blue for "Paid", Gray for neutral/empty states.
- **Selected Row**: A distinct light blue highlight to indicate selection.

### 3.3. Interactive Elements

- **Buttons & Controls**: All buttons ("New project", "Filter", etc.) and interactive elements should have clear hover and active states.
- **Collapsible Groups**: Each group header must feature an arrow icon that allows the user to collapse or expand the group's content.
- **Contextual Action Bar**: Upon selecting one or more items via the checkbox, a contextual action bar must appear at the bottom of the screen, providing options like "Duplicate", "Export", "Archive", "Delete", and "Move to".

## 4. Technical Requirements

- **Framework**: The component must be built using **React**.
- **Drag-and-Drop Library**: Utilize a modern and robust library for drag-and-drop functionality, such as `dnd-kit` or `React Beautiful DnD`, ensuring accessibility and mobile support.
- **State Management**: Employ a predictable state management solution (e.g., Redux, Zustand, or React Context) to handle the complex state of the board, including the nested structure of groups and items, selection state, and drag-and-drop operations.
- **Performance**: For boards with a large number of items, implement row virtualization to ensure smooth scrolling and a responsive user experience, preventing performance degradation.
- **Component Architecture**: Design the application with a modular component architecture. Create reusable components for elements like `Board`, `Group`, `Item`, `Column`, `StatusBadge`, and `TimelineBar`.

## 5. Acceptance Criteria

- [ ] The workspace component renders with the specified dark theme and layout.
- [ ] The board displays multiple, color-coded, collapsible groups.
- [ ] Users can drag and drop items to reorder them within a group.
- [ ] Users can drag and drop items to move them between different groups.
- [ ] Users can drag and drop entire groups to reorder them on the board.
- [ ] Drag-and-drop operations provide clear visual feedback (drag preview, drop indicators).
- [ ] Each group displays a summary row with accurate, real-time totals for all currency columns.
- [ ] All column types (Text, Timeline, Status, Currency) are rendered correctly according to the design specifications.
- [ ] Selecting items using checkboxes reveals a contextual action bar at the bottom of the page.
- [ ] The application remains performant and responsive when populated with a large number of items (e.g., 100+).
