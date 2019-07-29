# Input: TextArea

## Usage

```js
import { TextArea } from 'asc-web-components';
```

#### Description

TextArea is used for displaying custom textarea

#### Usage

```js
<TextArea placeholder="Add comment" onChange={event => alert(event.target.innerText)}>
  Some text
</TextArea>;
```

#### Properties

| Props                  | Type     | Required | Values                       | Default | Description                                                                                            |
| ---------------------- | -------- | :------: | ---------------------------- | ------- | ------------------------------------------------------------------------------------------------------ |
| `placeholder`                   | `string` |    -     | -                            | -       | Placeholder for TextArea                                                                             |
| `onChange`                 | `func` |    -     | -                            | -       | Allow you to handle changing events of component