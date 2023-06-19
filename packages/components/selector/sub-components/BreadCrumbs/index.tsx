import React from "react";

import ContextMenuButton from "../../../context-menu-button";

import {
  BreadCrumb,
  BreadCrumbsProps,
  DisplayedItem,
} from "./BreadCrumbs.types";

import {
  StyledBreadCrumbs,
  StyledItemText,
  StyledArrowRightSvg,
} from "./StyledBreadCrumbs";

const BreadCrumbs = ({
  breadCrumbs,
  onSelectBreadCrumb,
  isLoading,
}: BreadCrumbsProps) => {
  const [displayedItems, setDisplayedItems] = React.useState<DisplayedItem[]>(
    []
  );

  const onClickItem = React.useCallback(
    (e, open, item: BreadCrumb) => {
      if (isLoading) return;
      onSelectBreadCrumb && onSelectBreadCrumb(item);
    },
    [breadCrumbs, isLoading]
  );

  const calculateDisplayedItems = React.useCallback(
    (items: BreadCrumb[]) => {
      const itemsLength = items.length;
      const oldItems: BreadCrumb[] = [];

      items.forEach((item) =>
        oldItems.push({
          ...item,
          id: item.id.toString(),
        })
      );
      if (itemsLength > 0) {
        const newItems: DisplayedItem[] = [];

        if (itemsLength <= 3) {
          oldItems.forEach((item, index) => {
            newItems.push({
              ...item,
              isArrow: false,
              isList: false,
              listItems: [],
            });

            if (index !== oldItems.length - 1) {
              newItems.push({
                id: `arrow-${index}`,
                label: "",
                isArrow: true,
                isList: false,
                listItems: [],
              });
            }
          });
        } else {
          newItems.push({
            ...oldItems[0],
            isArrow: false,
            isList: false,
            listItems: [],
          });

          newItems.push({
            id: "arrow-1",
            label: "",
            isArrow: true,
            isList: false,
            listItems: [],
          });

          newItems.push({
            id: "drop-down-item",
            label: "",
            isArrow: false,
            isList: true,
            listItems: [],
          });

          newItems.push({
            id: "arrow-2",
            label: "",
            isArrow: true,
            isList: false,
            listItems: [],
          });

          newItems.push({
            ...oldItems[itemsLength - 2],
            isArrow: false,
            isList: false,
            listItems: [],
          });

          newItems.push({
            id: "arrow-3",
            label: "",
            isArrow: true,
            isList: false,
            listItems: [],
          });

          newItems.push({
            ...oldItems[itemsLength - 1],
            isArrow: false,
            isList: false,
            listItems: [],
          });

          oldItems.splice(0, 1);
          oldItems.splice(oldItems.length - 2, 2);

          oldItems.forEach((item) => {
            newItems[2].listItems?.push({
              ...item,
              minWidth: "150px",
              onClick: onClickItem,
            });
          });
        }

        return setDisplayedItems(newItems);
      }
    },
    [onClickItem]
  );

  React.useEffect(() => {
    if (breadCrumbs && breadCrumbs.length > 0) {
      calculateDisplayedItems(breadCrumbs);
    }
  }, [breadCrumbs, calculateDisplayedItems]);

  let gridTemplateColumns = "minmax(1px, max-content)";

  if (displayedItems.length > 5) {
    gridTemplateColumns =
      "minmax(1px, max-content) 12px 16px 12px minmax(1px, max-content) 12px minmax(1px, max-content)";
  } else if (displayedItems.length === 5) {
    gridTemplateColumns =
      "minmax(1px, max-content) 12px minmax(1px, max-content) 12px minmax(1px, max-content)";
  } else if (displayedItems.length === 3) {
    gridTemplateColumns =
      "minmax(1px, max-content) 12px minmax(1px, max-content)";
  }

  return (
    <StyledBreadCrumbs
      itemsCount={displayedItems.length}
      gridTemplateColumns={gridTemplateColumns}
    >
      {displayedItems.map((item, index) =>
        item.isList ? (
          <ContextMenuButton
            key={`bread-crumb-item-${item.id}-${index}`}
            className="context-menu-button"
            getData={() => item.listItems}
          />
        ) : item.isArrow ? (
          <StyledArrowRightSvg key={`bread-crumb-item-${item.id}-${index}`} />
        ) : (
          <StyledItemText
            key={`bread-crumb-item-${item.id}-${index}`}
            fontSize={"16px"}
            fontWeight={600}
            lineHeight={"22px"}
            noSelect
            truncate
            isCurrent={index === displayedItems.length - 1}
            isLoading={isLoading}
            onClick={() => {
              if (index === displayedItems.length - 1 || isLoading) return;

              onSelectBreadCrumb &&
                onSelectBreadCrumb({
                  id: item.id,
                  label: item.label,
                  isRoom: item.isRoom,
                });
            }}
          >
            {item.label}
          </StyledItemText>
        )
      )}
    </StyledBreadCrumbs>
  );
};

export default BreadCrumbs;
