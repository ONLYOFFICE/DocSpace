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
      visible={this.state.isOpen}
      onClose={() =>{}}
      onSave={(data) =>{console.log(data.isUpdate, data.croppedImageInfo)}}
      onImageChange={(data) =>{console.log(croppedImageInfo)}
    />
```

#### Properties

| Props                | Type         | Required | Values                           | Default                       | Description                                           |
| ------------------   | --------     | :------: | -------------------------------- | ----------------------------- | ----------------------------------------------------- |
| `visible`            | `bool`       |    -     |                                  | `false`                       | Display avatar editor or not                          |
| `image`              | `string/file`|    -     |                                  |                               | The URL of the image to use, or a File                |
| `accept`             | `array`      |    -     |                                  | `['image/png', 'image/jpeg']` | Accepted file types                                   |
| `displayType`        | `oneOf`      |    -     | `auto`, `modal`, `aside`         | `auto`                        |                                                       |
| `chooseFileLabel`    | `string`     |    -     |                                  | `Choose a file`               |                                                       |
| `headerLabel`        | `string`     |    -     |                                  | `Edit Photo`                  |                                                       |
| `saveButtonLabel`    | `string`     |    -     |                                  | `Save`                        |                                                       |
| `maxSizeFileError`   | `string`     |    -     |                                  | `Maximum file size exceeded`  |                                                       |
| `unknownTypeError`   | `string`     |    -     |                                  | `Unknown image file type`     |                                                       |
| `unknownError`       | `string`     |    -     |                                  | `Error`                       |                                                       |
| `maxSize`            | `number`     |    -     |                                  | `1`                           | Max size of image                                     |
| `onSave`             | `function`   |    -     |                                  |                               |                                                       |
| `onClose`            | `function`   |    -     |                                  |                               |                                                       |
| `onDeleteImage`      | `function`   |    -     |                                  |                               |                                                       |
| `onLoadFile`         | `function`   |    -     |                                  |                               |                                                       |
| `onImageChange`      | `function`   |    -     |                                  |                               |                                                       |