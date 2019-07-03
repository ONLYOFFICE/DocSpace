# ContentRow

## Usage

```js
import { ContentRow } from 'asc-web-components';
```

#### Description

Base ContentRow.

#### Usage

```js
<ContentRow checkBox='' avatar='' contextButton={optionData}>{rowItem}</ContentRow>
```

#### Properties

| Props              | Type     | Required | Values                           | Default   | Description                                               |
| ------------------ | -------- | :------: | -------------------------------- | --------- | --------------------------------------------------------- |
| `checkBox`         | `element`|    -     |                                  | ` `       | Required to host the Checkbox component. Its location is fixed and it is always the first. If there is no value, the occupied space is distributed among the other child elements.                                      |
| `avatar`           | `element`|    -     |                                  | ` `       | Required to host the Avatar component. It has a fixed order of location, if the Checkbox component is specified, then it follows, otherwise it occupies the first position. If there is no value, the occupied space is distributed among the other child elements.                                                 |
| `contextButton`    | `element`|    -     |                                  | ` `       | Required to host the ContextMenuButton component. It is always located near the right border of the container, regardless of the contents of the child elements. If there is no value, the occupied space is distributed among the other child elements.                             |