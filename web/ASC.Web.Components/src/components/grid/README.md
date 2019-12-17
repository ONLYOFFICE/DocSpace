# Grid 

A container that lays out its contents in a 2-dimensional grid system. Use Box components to define rows and columns.

### Usage

```js
import { Grid } from "asc-web-components";
```

```jsx
<Grid />
```

### Properties

| Props            |      Type         | Required | Values | Default | Description                                           |
| :--------------: | :---------------: | :------: | :----: | :-----: | ----------------------------------------------------- |
| `alignContent`   | `string`          |    -     |   -    |    -    | sets the distribution of space between and around content items along a flexbox's cross-axis or a grid's block axis |
| `alignItems`     | `string`          |    -     |   -    |    -    | sets the align-self value on all direct children as a group. In Flexbox, it controls the alignment of items on the Cross Axis. In Grid Layout, it controls the alignment of items on the Block Axis within their grid area. |
| `alignSelf`      | `string`          |    -     |   -    |    -    | overrides a grid or flex item's align-items value. In Grid, it aligns the item inside the grid area. In Flexbox, it aligns the item on the cross axis. |
| `areasProp`      | `string`,`object` |    -     |   -    |    -    | specifies named grid areas. |
| `columnsProp`    | `string`,`object` |    -     |   -    |    -    | defines the line names and track sizing functions of the grid columns. |
| `gridArea`       | `string`          |    -     |   -    |    -    | is a shorthand property for grid-row-start, grid-column-start, grid-row-end and grid-column-end, specifying a grid item’s size and location within the grid by contributing a line, a span, or nothing (automatic) to its grid placement, thereby specifying the edges of its grid area. |
| `gridColumnGap`  | `string`          |    -     |   -    |    -    | sets the size of the gap (gutter) between an element's columns. |
| `gridGap`        | `string`          |    -     |   -    |    -    | sets the gaps (gutters) between rows and columns. It is a shorthand for row-gap and column-gap. |
| `gridRowGap`     | `string`          |    -     |   -    |    -    | sets the size of the gap (gutter) between an element's grid rows. |
| `heightProp`     | `string`          |    -     |   -    | `100%`  | defines the height of the border of the element area. |
| `justifyContent` | `string`          |    -     |   -    |    -    | defines how the browser distributes space between and around content items along the main-axis of a flex container, and the inline axis of a grid container. |
| `justifyItems`   | `string`          |    -     |   -    |    -    | defines the default justify-self for all items of the box, giving them all a default way of justifying each box along the appropriate axis. |
| `justifySelf`    | `string`          |    -     |   -    |    -    | sets the way a box is justified inside its alignment container along the appropriate axis. |
| `marginProp`     | `string`          |    -     |   -    |    -    | sets the margin area on all four sides of an element. It is a shorthand for margin-top, margin-right, margin-bottom, and margin-left. |
| `paddingProp`    | `string`          |    -     |   -    |    -    | sets the padding area on all four sides of an element. It is a shorthand for padding-top, padding-right, padding-bottom, and padding-left |
| `rowsProp`       | `string`,`object` |    -     |   -    |    -    | defines the line names and track sizing functions of the grid rows. |
| `widthProp`      | `string`          |    -     |   -    | `100%`  | defines the width of the border of the element area. |
