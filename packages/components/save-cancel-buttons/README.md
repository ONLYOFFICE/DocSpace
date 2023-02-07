# SaveCancelButtons

Save and cancel buttons are located in the settings sections.

### Usage

```js
import SaveCancelButtons from "@docspace/components/save-cancel-button";
```

```jsx
<SaveCancelButtons
  onSaveClick={() => SomeFunction()}
  onCancelClick={() => SomeFunction()}
  showReminder={false}
  reminderTest="You have unsaved changes"
  saveButtonLabel="Save"
  cancelButtonLabel="Cancel"
/>
```

#### Properties

| Props                   |   Type   | Required | Values | Default  | Description                                      |
| ----------------------- | :------: | :------: | :----: | :------: | ------------------------------------------------ |
| `onSaveClick`           |  `func`  |    -     |   -    |    -     | What the save button will trigger when clicked   |
| `onCancelClick`         |  `func`  |    -     |   -    |    -     | What the cancel button will trigger when clicked |
| `reminderText`          | `string` |    -     |   -    |    -     | Text reminding of unsaved changes                |
| `showReminder`          |  `bool`  |    -     |   -    |    -     | Show message about unsaved changes.              |
| Only shown on desktops) |
| `saveButtonLabel`       | `string` |    -     |   -    |  `Save`  | Save button label                                |
| `cancelButtonLabel`     | `string` |    -     |   -    | `Cancel` | Cancel button label                              |
