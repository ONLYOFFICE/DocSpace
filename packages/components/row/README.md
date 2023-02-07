# Row

Displays content as row

### Usage

```js
import Row from "@docspace/components/row";
```

```jsx
<Row checked={false} contextOptions={[]}>
  {children}
</Row>
```

### Properties

| Props                      |      Type      | Required | Values | Default | Description                                                                                                                                                                                                                                                   |
| -------------------------- | :------------: | :------: | :----: | :-----: | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `checked`                  |     `bool`     |    -     |   -    |    -    | Required to host the Checkbox component. Its location is fixed and it is always the first. If there is no value, the occupied space is distributed among the other child elements.                                                                            |
| `className`                |    `string`    |    -     |   -    |    -    | Accepts class                                                                                                                                                                                                                                                 |
| `contextButtonSpacerWidth` |    `string`    |    -     |   -    | `32px`  | Required for the width task of the ContextMenuButton component.                                                                                                                                                                                               |
| `contextOptions`           |    `array`     |    -     |   -    |    -    | Required to host the ContextMenuButton component. It is always located near the right border of the container, regardless of the contents of the child elements. If there is no value, the occupied space is distributed among the other child elements.      |
| `data`                     |    `object`    |    -     |   -    |    -    | Current row item information.                                                                                                                                                                                                                                 |
| `element`                  |   `element`    |    -     |   -    |    -    | Required to host some component. It has a fixed order of location, if the Checkbox component is specified, then it follows, otherwise it occupies the first position. If there is no value, the occupied space is distributed among the other child elements. |
| `id`                       |    `string`    |    -     |   -    |    -    | Accepts id                                                                                                                                                                                                                                                    |
| `needForUpdate`            |   `function`   |    -     |   -    |    -    | Custom shouldComponentUpdate function                                                                                                                                                                                                                         |
| `onSelect`                 |   `function`   |    -     |   -    |    -    | Event when selecting row element. Returns data value.                                                                                                                                                                                                         |
| `style`                    | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                                                                                                                                                                                                                             |
