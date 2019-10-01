# ModalDialog

#### Description

ModalDialog is used for displaying modal dialogs

#### Usage

```js
import { ModalDialog } from 'asc-web-components';

<ModalDialog visible={false} />
```

#### Properties

| Props           | Type                      | Required | Values                   | Default | Description                                      |
| --------------- | ------------------------- | :------: | ------------------------ | ------- | ------------------------------------------------ |
| `visible`       | `bool`                    |          |                          |         | Display dialog or not                            |
| `displayType`   | `oneOf`                   |          | `auto`, `modal`, `aside` | `auto`  | Display type                                     |
| `scale`         | `bool`                    |          |                          |         | Indicates the side panel has scale               |
| `headerContent` | `string/element/elements` |          |                          |         | Header content                                   |
| `bodyContent`   | `string/element/elements` |          |                          |         | Body content                                     |
| `footerContent` | `string/element/elements` |          |                          |         | Footer content                                   |
| `onClose`       | `func`                    |          |                          |         | Will be triggered when a close button is clicked |