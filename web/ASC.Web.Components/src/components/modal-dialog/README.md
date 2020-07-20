# ModalDialog

ModalDialog is used for displaying modal dialogs

### Usage

```js
import { ModalDialog } from "asc-web-components";
```

```jsx
<ModalDialog
  visible={false}
  scale={false}
  displayType="auto"
  zIndex={310}
  headerContent="Change password"
  bodyContent={
    <div>
      Send the password change instruction to the{" "}
      <a href="mailto:asc@story.book">asc@story.book</a> email address
    </div>
  }
/>
```

### Properties

| Props           |        Type        | Required |          Values          | Default | Description                                      |
| --------------- | :----------------: | :------: | :----------------------: | :-----: | ------------------------------------------------ |
| `visible`       |       `bool`       |    -     |            -             |    -    | Display dialog or not                            |
| `displayType`   |      `oneOf`       |    -     | `auto`, `modal`, `aside` | `auto`  | Display type                                     |
| `scale`         |       `bool`       |    -     |            -             |    -    | Indicates the side panel has scale               |
| `headerContent` | `string`,`element` |    -     |            -             |    -    | Header content                                   |
| `bodyContent`   | `string`,`element` |    -     |            -             |    -    | Body content                                     |
| `footerContent` | `string`,`element` |    -     |            -             |    -    | Footer content                                   |
| `onClose`       |       `func`       |    -     |            -             |    -    | Will be triggered when a close button is clicked |
| `zIndex`        |      `number`      |    -     |            -             |  `310`  | CSS z-index                                      |
| `bodyPadding`        |      `string`      |    -     |            -             |  `16px 0`  | CSS padding props for body section                                      |
