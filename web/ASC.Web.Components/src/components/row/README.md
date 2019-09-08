# Row

## Usage

```js
import { Row } from 'asc-web-components';
```

#### Description

Displays content as row. 

#### Usage

```js
<Row 
    checked={false}
    contextOptions={[]}
>
    {children}
</Row>
```

#### Properties

| Props            | Type       | Required | Values | Default | Description                                               |
| ---------------- | ---------- | :------: | ------ | ------- | --------------------------------------------------------- |
| `checked`        | `bool`     | -        |        | ` `     | Required to host the Checkbox component. Its location is fixed and it is always the first. If there is no value, the occupied space is distributed among the other child elements. |
| `element`        | `element`  | -        |        | ` `     | Required to host some component. It has a fixed order of location, if the Checkbox component is specified, then it follows, otherwise it occupies the first position. If there is no value, the occupied space is distributed among the other child elements. |
| `contextOptions` | `array`    | -        |        | ` `     | Required to host the ContextMenuButton component. It is always located near the right border of the container, regardless of the contents of the child elements. If there is no value, the occupied space is distributed among the other child elements. |
| `data`           | `object`   | -        |        | ` `     | Current row item information.                              | 
| `onSelect`       | `function` | -        |        | ` `     | Event when selecting row element. Returns data value.      |