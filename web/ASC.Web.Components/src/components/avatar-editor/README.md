# Avatar Editor

## Usage

```js
import { AvatarEditor } from 'asc-web-components';
```

#### Description

Required to display user avatar editor on page.

#### Usage

```js
<AvatarEditor
  visible={true}
  onSave={(data) =>{console.log(data.croppedImage, data.defaultImage)}}
/>
```

#### Properties

| Props                | Type       | Required | Values                                    | Default            | Description                                           |
| ------------------   | --------   | :------: | ----------------------------------------- | ------------------ | ----------------------------------------------------- |
| `visible`            | `bool`     |    -     |                                           | `false`            | Display avatar editor or not                          |
| `chooseFileLabel`    | `string`   |    -     |                                           | `Choose a file`    |                                                       |
| `headerLabel`        | `string`   |    -     |                                           | `Edit Photo`       |                                                       |
| `saveButtonLabel`    | `string`   |    -     |                                           | `Save`             |                                                       |
| `cancelButtonLabel`  | `string`   |    -     |                                           | `Cancel`           |                                                       |
| `maxSize`            | `number`   |    -     |                                           | `1`                | Max size of image                                     |
| `onSave`             | `function` |    -     |                                           |                    |                                                       |
| `onClose`            | `function` |    -     |                                           |                    |                                                       |