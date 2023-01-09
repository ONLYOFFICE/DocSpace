# SelectedItem

### Usage

```js
import Selector from "@docspace/components/selector";
```

```jsx
<Selector
  acceptButtonLabel="Add"
  accessRights={[]}
  cancelButtonLabel="Cancel"
  emptyScreenDescription="The list of users previously invited to DocSpace or separate rooms will appear here. You will be able to invite these users for collaboration at any time."
  emptyScreenHeader="No other accounts here yet"
  emptyScreenImage="static/images/empty_screen_filter.png"
  hasNextPage={false}
  headerLabel="Room list"
  items={[]}
  isLoading={false}
  loadNextPage={() => {}}
  onAccept={function noRefCheck() {}}
  onAccessRightsChange={function noRefCheck() {}}
  onBackClick={function noRefCheck() {}}
  onCancel={function noRefCheck() {}}
  onClearSearch={function noRefCheck() {}}
  onSearch={function noRefCheck() {}}
  onSelect={function noRefCheck() {}}
  onSelectAll={function noRefCheck() {}}
  rowLoader={<></>}
  searchEmptyScreenDescription=" SEARCH !!! The list of users previously invited to DocSpace or separate rooms will appear here. You will be able to invite these users for collaboration at any time."
  searchEmptyScreenHeader="No other accounts here yet search"
  searchEmptyScreenImage="static/images/empty_screen_filter.png"
  searchLoader={<></>}
  searchPlaceholder="Search"
  searchValue=""
  selectAllIcon="static/images/room.archive.svg"
  selectAllLabel="All accounts"
  selectedAccessRight={{}}
  selectedItems={[]}
  totalItems={0}
/>
```

### Properties

| Props                          |      Type      | Required | Values | Default | Description                                                   |
| ------------------------------ | :------------: | :------: | :----: | :-----: | ------------------------------------------------------------- |
| `id`                           |    `string`    |    -     |   -    |    -    | Accepts id                                                    |
| `className`                    |    `string`    |    -     |   -    |    -    | Accepts class                                                 |
| `style`                        | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                             |
| `headerLabel`                  |    `string`    |    -     |   -    |    -    | Selector header text                                          |
| `onBackClick`                  |     `func`     |    -     |   -    |    -    | What the header arrow will trigger when clicked               |
| `searchPlaceholder`            |    `string`    |    -     |   -    |    -    | Placeholder for search input                                  |
| `onSearch`                     |     `func`     |    -     |   -    |    -    | What the search input will trigger when user stopped typing   |
| `onClearSearch`                |     `func`     |    -     |   -    |    -    | What the clear icon of search input will trigger when clicked |
| `items`                        |    `array`     |    -     |   -    |    -    | Displaying items                                              |
| `onSelect`                     |     `func`     |    -     |   -    |    -    | What the item will trigger when clicked                       |
| `isMultiSelect`                |     `bool`     |    -     |   -    |  false  | Allows you to select multiple objects                         |
| `selectedItems`                |    `array`     |    -     |   -    |   []    | Tells when the items should present a checked state           |
| `acceptButtonLabel`            |    `string`    |    -     |   -    |    -    | Accept button text                                            |
| `onAccept`                     |     `func`     |    -     |   -    |    -    | What the accept button will trigger when clicked              |
| `withSelectAll`                |     `bool`     |    -     |   -    |  false  | Add option for select all items                               |
| `selectAllLabel`               |    `string`    |    -     |   -    |    -    | Text for select all component                                 |
| `selectAllIcon`                |    `string`    |    -     |   -    |    -    | Icon for select all component                                 |
| `onSelectAll`                  |     `func`     |    -     |   -    |    -    | What the select all will trigger when clicked                 |
| `withAccessRights`             |     `bool`     |    -     |   -    |  false  | Add combobox for displaying access rights at footer           |
| `accessRights`                 |    `array`     |    -     |   -    |    -    | Access rights items                                           |
| `selectedAccessRight`          |    `object`    |    -     |   -    |    -    | Selected access rights items                                  |
| `onAccessRightsChange`         |     `func`     |    -     |   -    |    -    | What the access right will trigger when changed               |
| `withCancelButton`             |     `bool`     |    -     |   -    |  false  | Add cancel button at footer                                   |
| `cancelButtonLabel`            |    `string`    |    -     |   -    |    -    | Displaying text at cancel button                              |
| `onCancel`                     |     `func`     |    -     |   -    |    -    | What the cancel button will trigger when clicked              |
| `emptyScreenImage`             |    `string`    |    -     |   -    |    -    | Image for default empty screen                                |
| `emptyScreenHeader`            |    `string`    |    -     |   -    |    -    | Header for default empty screen                               |
| `emptyScreenDescription`       |    `string`    |    -     |   -    |    -    | Description for default empty screen                          |
| `searchEmptyScreenImage`       |    `string`    |    -     |   -    |    -    | Image for search empty screen                                 |
| `searchEmptyScreenHeader`      |    `string`    |    -     |   -    |    -    | Header for search empty screen                                |
| `searchEmptyScreenDescription` |    `string`    |    -     |   -    |    -    | Description for search empty screen                           |
| `totalItems`                   |    `number`    |    -     |   -    |    -    | Count items for infinity scroll                               |
| `hasNextPage`                  |     `bool`     |    -     |   -    |    -    | Has next page for infinity scroll                             |
| `isNextPageLoading`            |     `bool`     |    -     |   -    |    -    | Tells when next page already loading                          |
| `loadNextPage`                 |     `func`     |    -     |   -    |    -    | Function for load next page                                   |
| `isLoading`                    |     `bool`     |    -     |   -    |    -    | Set loading state for select                                  |
| `searchLoader`                 |     `node`     |    -     |   -    |    -    | Loader element for search block                               |
| `rowLoader`                    |     `node`     |    -     |   -    |    -    | Loader element for item                                       |
