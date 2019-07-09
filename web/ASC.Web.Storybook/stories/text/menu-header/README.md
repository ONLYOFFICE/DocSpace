# Text

## Usage

```js
import { Text } from 'asc-web-components';
```

### <Text.MenuHeader>

Wraps the given text in the specified size of the menu header.

#### Usage

```js
    <Text.MenuHeader title='Some title' isInline>
        Some text
    </Text.MenuHeader>
```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `isDisabled`       | `bool`   |    -     | -                     | false     | Marks text as disabled                              |
| `title`            | `bool`   |    -     | -                     | -         | Title                                               |
| `truncate`         | `bool`   |    -     | -                     | false     | Disables word wrapping                              |
| `isInline`         | `bool`   |    -     | -                     | false     | Sets the 'display: inline-block' property           |