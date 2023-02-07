# TreeMenu

Tree menu description

### Usage

```js
import TreeMenu from "@docspace/components/tree-menu";
```

```jsx
<TreeMenu data={data} />
```

### Properties TreeMenu

| Props                   |      Type      | Required | Values | Default | Description                                                                                             |
| ----------------------- | :------------: | :------: | :----: | :-----: | ------------------------------------------------------------------------------------------------------- |
| `autoExpandParent`      |     `bool`     |    -     |   -    | `false` | Whether auto expand parent treeNodes                                                                    |
| `checkable`             |     `bool`     |    -     |   -    | `false` | Whether support checked                                                                                 |
| `className`             |    `string`    |    -     |   -    |    -    | Accepts class                                                                                           |
| `defaultExpandAll`      |     `bool`     |    -     |   -    | `false` | Expand all treeNodes                                                                                    |
| `defaultExpandParent`   |     `bool`     |    -     |   -    | `true`  | Auto expand parent treeNodes when init                                                                  |
| `disabled`              |     `bool`     |    -     |   -    | `false` | Whether disabled the tree                                                                               |
| `draggable`             |     `bool`     |    -     |   -    | `false` | Whether can drag treeNode                                                                               |
| `id`                    |    `string`    |    -     |   -    |    -    | Accepts id                                                                                              |
| `multiple`              |     `bool`     |    -     |   -    | `false` | Whether multiple select                                                                                 |
| `onDragEnd`             |     `func`     |    -     |   -    |    -    | it execs when fire the tree's dragend event                                                             |
| `onDragEnter`           |     `func`     |    -     |   -    |    -    | it execs when fire the tree's dragenter event                                                           |
| `onDragLeave`           |     `func`     |    -     |   -    |    -    | it execs when fire the tree's dragleave event                                                           |
| `onDragOver`            |     `func`     |    -     |   -    |    -    | it execs when fire the tree's dragover event                                                            |
| `onDragStart`           |     `func`     |    -     |   -    |    -    | it execs when fire the tree's dragstart event                                                           |
| `onDrop`                |     `func`     |    -     |   -    |    -    | it execs when fire the tree's drop event                                                                |
| `onExpand`              |     `func`     |    -     |   -    |    -    | fire on treeNode expand or not                                                                          |
| `onLoad`                |     `func`     |    -     |   -    |    -    | Trigger when a node is loaded. If you set the loadedKeys, you must handle onLoad to avoid infinity loop |
| `onMouseEnter`          |     `func`     |    -     |   -    |    -    | call when mouse enter a treeNode                                                                        |
| `onMouseLeave`          |     `func`     |    -     |   -    |    -    | call when mouse leave a treeNode                                                                        |
| `onRightClick`          |     `func`     |    -     |   -    |    -    | select current treeNode and show customized contextmenu                                                 |
| `onSelect`              |     `func`     |    -     |   -    |    -    | click the treeNode to fire                                                                              |
| `showIcon`              |     `bool`     |    -     |   -    | `true`  | Whether show icon                                                                                       |
| `showLine`              |     `bool`     |    -     |   -    | `false` | Whether show line                                                                                       |
| `style`                 | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                                                                       |
| `loadData`              |     `func`     |    -     |   -    |    -    | load data asynchronously and the return value should be a promise                                       |
| `isFullFillSelection`   |     `bool`     |    -     |   -    | `true`  | to select the selection style of the active node                                                        |
| `gapBetweenNodes`       |    `string`    |    -     |   -    |  `15`   | for setting the spacing between nodes                                                                   |
| `gapBetweenNodesTablet` |    `string`    |    -     |   -    |    -    | to set spacing between nodes on tablets and phones (if necessary)                                       |
| `isEmptyRootNode`       |     `bool`     |    -     |   -    | `false` | swipe the root node to the left if there are no nested elements                                         |

### Properties TreeNode

| Props             |        Type        | Required | Values | Default | Description                                                                                                                                                                 |
| ----------------- | :----------------: | :------: | :----: | :-----: | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `checkable`       |       `bool`       |    -     |   -    |    -    | control node checkable if Tree is checkable                                                                                                                                 |
| `className`       |      `string`      |    -     |   -    |    -    | additional class to treeNode                                                                                                                                                |
| `disableCheckbox` |       `bool`       |    -     |   -    | `false` | whether disable the treeNode' checkbox                                                                                                                                      |
| `disabled`        |       `bool`       |    -     |   -    | `false` | whether disabled the treeNode                                                                                                                                               |
| `icon`            |  `element`,`func`  |    -     |   -    | `false` | customize icon. When you pass component, whose render will receive full TreeNode props as component props                                                                   |
| `isLeaf`          |       `bool`       |    -     |   -    | `false` | whether it's leaf node                                                                                                                                                      |
| `key`             |       `bool`       |    -     |   -    |    -    | it's used with tree props's (default)ExpandedKeys / (default)CheckedKeys / (default)SelectedKeys. you'd better to set it, and it must be unique in the tree's all treeNodes |
| `style`           |      `object`      |    -     |   -    |    -    | set style to treeNode                                                                                                                                                       |
| `title`           | `string`,`element` |    -     |   -    |    -    | tree/subTree's title                                                                                                                                                        |
| `newItems`        |      `number`      |    -     |   -    |    -    | The number of new elements in the node                                                                                                                                      |
| `onBadgeClick`    |       `func`       |    -     |   -    |    -    | call when click on badge                                                                                                                                                    |
| `showBadge`       |       `bool`       |    -     |   -    |    -    | to display the badge                                                                                                                                                        |
