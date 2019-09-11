# RowContainer

## Usage

```js
import { RowContainer } from "asc-web-components";
```

#### Description

Container for rows

#### Usage

```js
<RowContainer manualHeight="500px">{children}</RowContainer>
```

#### Properties

| Props          | Type     | Required | Values | Default | Description                                                     |
| -------------- | -------- | :------: | ------ | ------- | --------------------------------------------------------------- |
| `manualHeight` | `string` |    -     |        | -       | Allows you to set fixed block height for Row                    |
| `itemHeight`   | `number` |    -     |        | 50      | Height of one Row element. Required for scroll to work properly |
