# RowContent

## Usage

```js
import { RowContent } from "asc-web-components";
```

#### Description

Required for formatted output of elements inside Row.

To correctly display components inside RowContent, you must specify them in a certain order.

The first and second specified components will be interpreted as Main elements.
First will be MainTitle and second MainIcons.
All subsequent components will be located on the right and are considered SideElements.

**_Consider location of components in advance, since when viewing in tablet mode, the markup will shift SideElements to second line._**

Each not main child can take containerWidth property for task of width of child's container.

#### Usage

```js
<RowContent>
  <Link type="page" title="Demo" isBold={true} fontSize={15} color="#333333">
    Demo
  </Link>
  <>
    <Icons.SendClockIcon size="small" isfill={true} color="#3B72A7" />
    <Icons.CatalogSpamIcon size="small" isfill={true} color="#3B72A7" />
  </>
  <Link type="page" title="Demo" fontSize={12} color="#A3A9AE">
    Demo
  </Link>
  <Link
    containerWidth="160px"
    type="action"
    title="Demo"
    fontSize={12}
    color="#A3A9AE"
  >
    Demo
  </Link>
  <Link type="page" title="0 000 0000000" fontSize={12} color="#A3A9AE">
    0 000 0000000
  </Link>
  <Link
    containerWidth="160px"
    type="page"
    title="demo@demo.com"
    fontSize={12}
    color="#A3A9AE"
  >
    demo@demo.com
  </Link>
</RowContent>
```

#### Properties

| Props             | Type     | Required | Values | Default | Description                            |
| ----------------- | -------- | :------: | ------ | ------- | -------------------------------------- |
| `children`        | `node`   |    ✅    | -      | -       | Components displayed inside RowContent |
| `containerWidth`  | `string` |    -     | -      | `100px` | For task of width of child's container |
| `disableSideInfo` | `bool`   |    -     | -      | `false` | If you do not need SideElements        |
