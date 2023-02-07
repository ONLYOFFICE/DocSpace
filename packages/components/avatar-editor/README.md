# Avatar Editor

Used to display user avatar editor on page.

### Usage

```js
import AvatarEditor from "@docspace/components/avatar-editor";
```

```jsx
<AvatarEditor
  visible={true}
  onClose={() => {}}
  onSave={() => {})}
  onDeleteImage={() => {})}
  onImageChange={() => {})}
  onLoadFile={() => {}}
  headerLabel="Edit Photo"
  selectNewPhotoLabel="Select new photo"
  orDropFileHereLabel="or drop file here"
  saveButtonLabel="Save"
  maxSizeFileError="Maximum file size exceeded"
  unknownTypeError="Unknown image file type"
  unknownError="Error"
  displayType="auto"
/>
```

### Properties

| Props                 |      Type       | Required |          Values          |            Default            | Description                                                                 |
| --------------------- | :-------------: | :------: | :----------------------: | :---------------------------: | --------------------------------------------------------------------------- |
| `visible`             |     `bool`      |    -     |            -             |            `false`            | Display avatar editor                                                       |
| `image`               | `string`,`file` |    -     |            -             |               -               | The URL of the image to use, or a File                                      |
| `accept`              |     `array`     |    -     |            -             | `['image/png', 'image/jpeg']` | Accepted file types                                                         |
| `displayType`         |     `oneOf`     |    -     | `auto`, `modal`, `aside` |            `auto`             | Display type                                                                |
| `selectNewPhotoLabel` |    `string`     |    -     |            -             |      `Select new photo`       | Translation string for file selection                                       |
| `orDropFileHereLabel` |    `string`     |    -     |            -             |      `or drop file here`      | Translation string for file dropping (concat with selectNewPhotoLabel prop) |
| `headerLabel`         |    `string`     |    -     |            -             |         `Edit Photo`          | Translation string for title                                                |
| `saveButtonLabel`     |    `string`     |    -     |            -             |            `Save`             | Translation string for save button                                          |
| `saveButtonLoading`   |     `bool`      |    -     |            -             |               -               | Tells when the button should show loader icon                               |
| `maxSizeFileError`    |    `string`     |    -     |            -             | `Maximum file size exceeded`  | Translation string for size warning                                         |
| `unknownTypeError`    |    `string`     |    -     |            -             |   `Unknown image file type`   | Translation string for file type warning                                    |
| `unknownError`        |    `string`     |    -     |            -             |            `Error`            | Translation string for warning                                              |
| `maxSize`             |    `number`     |    -     |            -             |              `1`              | Max size of image                                                           |
| `onSave`              |   `function`    |    -     |            -             |               -               | Save event                                                                  |
| `onClose`             |   `function`    |    -     |            -             |               -               | Closing event                                                               |
| `onDeleteImage`       |   `function`    |    -     |            -             |               -               | Image deletion event                                                        |
| `onLoadFile`          |   `function`    |    -     |            -             |               -               | Image upload event                                                          |
| `onImageChange`       |   `function`    |    -     |            -             |               -               | Image change event                                                          |
| `className`           |    `string`     |    -     |            -             |               -               | Accepts class                                                               |
| `id`                  |    `string`     |    -     |            -             |               -               | Accepts id                                                                  |
| `style`               | `obj`, `array`  |    -     |            -             |               -               | Accepts css style                                                           |
