# ModalDialog

#### Description

ModalDialog is used for displaying modal dialogs

#### Usage

```js
import { ModalDialog } from 'asc-web-components';

<ModalDialog visible={false} />
```

#### Properties

| Props           | Type                      | Required | Values | Default | Description                                      |
| --------------- | ------------------------- | :------: | -------| ------- | ------------------------------------------------ |
| `visible`       | `bool`                    |          |        |         | Display dialog or not                            |
| `headerContent` | `string/element/elements` |          |        |         | Header content                                   |
| `bodyContent`   | `string/element/elements` |          |        |         | Body content                                     |
| `footerContent` | `string/element/elements` |          |        |         | Footer content                                   |
| `zIndex`        | `number`                  |          |        |   310   | CSS z-index                                      |
| `onClose`       | `func`                    |          |        |         | Will be triggered when a close button is clicked |