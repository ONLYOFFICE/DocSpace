# ModalDialog

ModalDialog is used for displaying modal dialogs

### Usage

```js
import ModalDialog from "@docspace/components/modal-dialog";
```

```jsx
<ModalDialog visible={false} scale={false} displayType="auto" zIndex={310}>
  <ModalDialog.Header>"Change password"</ModalDialog.Header>
  <ModalDialog.Body>
    <div>
      Send the password change instruction to the{" "}
      <a href="mailto:asc@story.book">asc@story.book</a> email address
    </div>
  </ModalDialog.Body>
  <ModalDialog.Footer>
    <Button
      label="Send"
      size="big"
      primary={true}
      onClick={() => alert("It's works!")}
    />
  </ModalDialog.Footer>
</ModalDialog>
```

### Properties

| Props              |   Type   | Required |          Values          | Default  | Description                                      |
| ------------------ | :------: | :------: | :----------------------: | :------: | ------------------------------------------------ |
| `visible`          |  `bool`  |    -     |            -             |    -     | Display dialog or not                            |
| `displayType`      | `oneOf`  |    -     | `auto`, `modal`, `aside` |  `auto`  | Display type                                     |
| `scale`            |  `bool`  |    -     |            -             |    -     | Indicates the side panel has scale               |
| `onClose`          |  `func`  |    -     |            -             |    -     | Will be triggered when a close button is clicked |
| `zIndex`           | `number` |    -     |            -             |  `310`   | CSS z-index                                      |
| `modalBodyPadding` | `string` |    -     |            -             | `16px 0` | CSS padding props for modal body section         |
| `asideBodyPadding` | `string` |    -     |            -             | `16px 0` | CSS padding props for aside body section         |
