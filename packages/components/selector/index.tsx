import React from "react";
import PropTypes from "prop-types";

import Header from "./sub-components/Header";
import Body from "./sub-components/Body";
import Footer from "./sub-components/Footer";

import { StyledSelector } from "./StyledSelector";

import { AccessRight, SelectorProps } from "./Selector.types";
import { Item } from "./sub-components/Item/Item.types";

const Selector = ({
  id,
  className,
  style,

  headerLabel,
  withoutBackButton,
  onBackClick,

  isBreadCrumbsLoading,
  breadCrumbsLoader,
  withBreadCrumbs,
  breadCrumbs,
  onSelectBreadCrumb,

  withSearch,
  searchLoader,
  searchPlaceholder,
  searchValue,
  onSearch,
  onClearSearch,

  withSelectAll,
  selectAllLabel,
  selectAllIcon,
  onSelectAll,

  items,
  isMultiSelect,
  selectedItems,
  acceptButtonLabel,
  onSelect,
  onAccept,
  rowLoader,

  withAccessRights,
  accessRights,
  selectedAccessRight,
  onAccessRightsChange,

  withCancelButton,
  cancelButtonLabel,
  onCancel,

  emptyScreenImage,
  emptyScreenHeader,
  emptyScreenDescription,

  searchEmptyScreenImage,
  searchEmptyScreenHeader,
  searchEmptyScreenDescription,

  hasNextPage,
  isNextPageLoading,
  loadNextPage,
  totalItems,
  isLoading,

  withHeader,

  withFooterInput,
  withFooterCheckbox,
  footerInputHeader,
  footerCheckboxLabel,
  currentFooterInputValue,

  alwaysShowFooter,
  disableAcceptButton,

  descriptionText,
  acceptButtonId,
  cancelButtonId,
}: SelectorProps) => {
  const [footerVisible, setFooterVisible] = React.useState<boolean>(false);
  const [isSearch, setIsSearch] = React.useState<boolean>(false);

  const [renderedItems, setRenderedItems] = React.useState<Item[]>([]);
  const [newSelectedItems, setNewSelectedItems] = React.useState<Item[]>([]);

  const [newFooterInputValue, setNewFooterInputValue] = React.useState<string>(
    currentFooterInputValue || ""
  );
  const [isFooterCheckboxChecked, setIsFooterCheckboxChecked] =
    React.useState<boolean>(false);

  const [selectedAccess, setSelectedAccess] =
    React.useState<AccessRight | null>(null);

  const onBackClickAction = React.useCallback(() => {
    onBackClick && onBackClick();
  }, [onBackClick]);

  const onSearchAction = React.useCallback(
    (value: string) => {
      onSearch && onSearch(value);

      setIsSearch(true);
    },
    [onSearch]
  );

  const onClearSearchAction = React.useCallback(() => {
    onClearSearch && onClearSearch();
    setIsSearch(false);
  }, [onClearSearch]);

  const onSelectAction = (item: Item) => {
    onSelect &&
      onSelect({
        ...item,
        id: item.id,
        email: item.email || "",
        avatar: item.avatar,
        icon: item.icon,
        label: item.label,
      });

    if (isMultiSelect) {
      setRenderedItems((value) => {
        const idx = value.findIndex((x) => item.id === x.id);

        const newValue = value.map((item) => ({ ...item }));

        if (idx === -1) return newValue;

        newValue[idx].isSelected = !value[idx].isSelected;

        return newValue;
      });

      if (item.isSelected) {
        setNewSelectedItems((value) => {
          const newValue = value
            .filter((x) => x.id !== item.id)
            .map((x) => ({ ...x }));
          compareSelectedItems(newValue);
          return newValue;
        });
      } else {
        setNewSelectedItems((value) => {
          value.push({
            id: item.id,
            email: item.email,
            ...item,
          });

          compareSelectedItems(value);

          return value;
        });
      }
    } else {
      setRenderedItems((value) => {
        const idx = value.findIndex((x) => item.id === x.id);

        const newValue = value.map((item) => ({ ...item, isSelected: false }));

        if (idx === -1) return newValue;

        newValue[idx].isSelected = !item.isSelected;

        return newValue;
      });

      const newItem = {
        id: item.id,
        email: item.email,

        ...item,
      };

      if (item.isSelected) {
        setNewSelectedItems([]);
        compareSelectedItems([]);
      } else {
        setNewSelectedItems([newItem]);
        compareSelectedItems([newItem]);
      }
    }
  };

  const onSelectAllAction = React.useCallback(() => {
    onSelectAll && onSelectAll();
    if (newSelectedItems.length === 0) {
      const cloneItems = items.map((x) => ({ ...x }));

      const cloneRenderedItems = items.map((x) => ({ ...x, isSelected: true }));

      setRenderedItems(cloneRenderedItems);
      setNewSelectedItems(cloneItems);
      compareSelectedItems(cloneItems);
    } else {
      const cloneRenderedItems = items.map((x) => ({
        ...x,
        isSelected: false,
      }));

      setRenderedItems(cloneRenderedItems);
      setNewSelectedItems([]);
      compareSelectedItems([]);
    }
  }, [items, newSelectedItems]);

  const onAcceptAction = () => {
    onAccept &&
      onAccept(
        newSelectedItems,
        selectedAccess,
        newFooterInputValue,
        isFooterCheckboxChecked
      );
  };

  const onCancelAction = React.useCallback(() => {
    onCancel && onCancel();
  }, [onCancel]);

  const onChangeAccessRightsAction = React.useCallback(
    (access: AccessRight) => {
      setSelectedAccess({ ...access });
      onAccessRightsChange && onAccessRightsChange(access);
    },
    [onAccessRightsChange]
  );

  const compareSelectedItems = React.useCallback(
    (newList: Item[]) => {
      let isEqual = true;

      if (selectedItems?.length !== newList.length) {
        return setFooterVisible(true);
      }

      if (newList.length === 0 && selectedItems?.length === 0) {
        return setFooterVisible(false);
      }

      newList.forEach((item) => {
        isEqual = selectedItems.some((x) => x.id === item.id);
      });

      return setFooterVisible(!isEqual);
    },
    [selectedItems]
  );

  const loadMoreItems = React.useCallback(
    (startIndex: number) => {
      !isNextPageLoading && loadNextPage && loadNextPage(startIndex - 1);
    },
    [isNextPageLoading, loadNextPage]
  );

  React.useEffect(() => {
    if (!!selectedAccessRight) setSelectedAccess({ ...selectedAccessRight });
  }, [selectedAccessRight]);

  React.useEffect(() => {
    if (items && selectedItems) {
      if (selectedItems.length === 0 || !isMultiSelect) {
        const cloneItems = items.map((x) => ({ ...x, isSelected: false }));
        return setRenderedItems(cloneItems);
      }

      const newItems = items.map((item) => {
        const { id } = item;

        const isSelected = selectedItems.some(
          (selectedItem) => selectedItem.id === id
        );

        return { ...item, isSelected };
      });

      const cloneSelectedItems = selectedItems.map((item) => ({ ...item }));

      setRenderedItems(newItems);
      setNewSelectedItems(cloneSelectedItems);
      compareSelectedItems(cloneSelectedItems);
    }
  }, [items, selectedItems, isMultiSelect, compareSelectedItems]);

  return (
    <StyledSelector id={id} className={className} style={style}>
      {withHeader && (
        <Header
          onBackClickAction={onBackClickAction}
          headerLabel={headerLabel}
          withoutBackButton={withoutBackButton}
        />
      )}

      <Body
        withHeader={withHeader}
        footerVisible={footerVisible || !!alwaysShowFooter}
        isSearch={isSearch}
        isAllIndeterminate={
          newSelectedItems.length !== renderedItems.length &&
          newSelectedItems.length !== 0
        }
        isAllChecked={
          newSelectedItems.length === renderedItems.length &&
          renderedItems.length !== 0
        }
        placeholder={searchPlaceholder}
        value={searchValue}
        onSearch={onSearchAction}
        onClearSearch={onClearSearchAction}
        items={renderedItems}
        isMultiSelect={isMultiSelect}
        onSelect={onSelectAction}
        withSelectAll={withSelectAll}
        selectAllLabel={selectAllLabel}
        selectAllIcon={selectAllIcon}
        onSelectAll={onSelectAllAction}
        emptyScreenImage={emptyScreenImage}
        emptyScreenHeader={emptyScreenHeader}
        emptyScreenDescription={emptyScreenDescription}
        searchEmptyScreenImage={searchEmptyScreenImage}
        searchEmptyScreenHeader={searchEmptyScreenHeader}
        searchEmptyScreenDescription={searchEmptyScreenDescription}
        hasNextPage={hasNextPage}
        isNextPageLoading={isNextPageLoading}
        loadMoreItems={loadMoreItems}
        totalItems={totalItems}
        isLoading={isLoading}
        searchLoader={searchLoader}
        rowLoader={rowLoader}
        withBreadCrumbs={withBreadCrumbs}
        breadCrumbs={breadCrumbs}
        onSelectBreadCrumb={onSelectBreadCrumb}
        breadCrumbsLoader={breadCrumbsLoader}
        isBreadCrumbsLoading={isBreadCrumbsLoading}
        withSearch={withSearch}
        withFooterInput={withFooterInput}
        withFooterCheckbox={withFooterCheckbox}
        descriptionText={descriptionText}
      />

      {(footerVisible || alwaysShowFooter) && (
        <Footer
          isMultiSelect={isMultiSelect}
          acceptButtonLabel={acceptButtonLabel}
          selectedItemsCount={newSelectedItems.length}
          withCancelButton={withCancelButton}
          cancelButtonLabel={cancelButtonLabel}
          withAccessRights={withAccessRights}
          accessRights={accessRights}
          selectedAccessRight={selectedAccess}
          onAccept={onAcceptAction}
          onCancel={onCancelAction}
          onChangeAccessRights={onChangeAccessRightsAction}
          withFooterInput={withFooterInput}
          withFooterCheckbox={withFooterCheckbox}
          footerInputHeader={footerInputHeader}
          footerCheckboxLabel={footerCheckboxLabel}
          currentFooterInputValue={newFooterInputValue}
          setNewFooterInputValue={setNewFooterInputValue}
          isFooterCheckboxChecked={isFooterCheckboxChecked}
          setIsFooterCheckboxChecked={setIsFooterCheckboxChecked}
          disableAcceptButton={
            withFooterInput
              ? disableAcceptButton
              : disableAcceptButton && !newFooterInputValue.trim()
          }
          acceptButtonId={acceptButtonId}
          cancelButtonId={cancelButtonId}
        />
      )}
    </StyledSelector>
  );
};

// Selector.propTypes = {
//   /** Accepts id  */
//   id: PropTypes.string,
//   /** Accepts class  */
//   className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
//   /** Accepts css style */
//   style: PropTypes.object,

/** Add header */
// withHeader: PropTypes.bool,
//   /** Selector header text */
//   headerLabel: PropTypes.string,
//   /** Hide header back button */
//   withoutBackButton: PropTypes.bool,
//   /** Sets a callback function that is triggered when the header arrow is clicked */
//   onBackClick: PropTypes.func,

//   /** Placeholder for search input */
//   searchPlaceholder: PropTypes.string,
//   /** Start value for search input */
//   searchValue: PropTypes.string,
//   /** Sets a callback function that is triggered when the user stops typing */
//   onSearch: PropTypes.func,
//   /** Sets a callback function that is triggered when the clear icon of the search input is clicked  */
//   onClearSearch: PropTypes.func,

//   /** Displays items */
//   items: PropTypes.array,
//   /** Sets a callback function that is triggered when the item is clicked */
//   onSelect: PropTypes.func,

//   /** Allows selecting multiple objects */
//   isMultiSelect: PropTypes.bool,
//   /** Sets the items to present a checked state */
//   selectedItems: PropTypes.array,
//   /** Accepts button text */
//   acceptButtonLabel: PropTypes.string,
//   /** Sets a callback function that is triggered when the accept button is clicked */
//   onAccept: PropTypes.func,

//   /** Adds an option for selecting all items */
//   withSelectAll: PropTypes.bool,
//   /** Text for selecting all components */
//   selectAllLabel: PropTypes.string,
//   /** Icon for selecting all components */
//   selectAllIcon: PropTypes.string,
//   /** Sets a callback function that is triggered when SelectAll is clicked */
//   onSelectAll: PropTypes.func,

//   /** Adds combobox for displaying access rights at the footer */
//   withAccessRights: PropTypes.bool,
//   /** Access rights items */
//   accessRights: PropTypes.array,
//   /** Selected access rights items */
//   selectedAccessRight: PropTypes.object,
//   /** Sets a callback function that is triggered when the access rights are changed */
//   onAccessRightsChange: PropTypes.func,

//   /** Adds a cancel button at the footer */
//   withCancelButton: PropTypes.bool,
//   /** Displays text in the cancel button */
//   cancelButtonLabel: PropTypes.string,
//   /** Sets a callback function that is triggered when the cancel button is clicked */
//   onCancel: PropTypes.func,

//   /** Image for default empty screen */
//   emptyScreenImage: PropTypes.string,
//   /** Header for default empty screen */
//   emptyScreenHeader: PropTypes.string,
//   /** Description for default empty screen */
//   emptyScreenDescription: PropTypes.string,

//   /** Image for search empty screen */
//   searchEmptyScreenImage: PropTypes.string,
//   /** Header for search empty screen */
//   searchEmptyScreenHeader: PropTypes.string,
//   /** Description for search empty screen */
//   searchEmptyScreenDescription: PropTypes.string,

//   /** Counts items for infinity scroll */
//   totalItems: PropTypes.number,
//   /** Sets the next page for the infinity scroll */
//   hasNextPage: PropTypes.bool,
//   /** Notifies that the next page is loading */
//   isNextPageLoading: PropTypes.bool,
//   /** Sets a callback function that is invoked to load the next page */
//   loadNextPage: PropTypes.func,

//   /** Sets loading state for select */
//   isLoading: PropTypes.bool,
//   /** Loader element for search block */
//   searchLoader: PropTypes.node,
//   /** Loader element for item */
//   rowLoader: PropTypes.node,
// };

Selector.defaultProps = {
  isMultiSelect: false,
  withSelectAll: false,
  withAccessRights: false,
  withCancelButton: false,
  withoutBackButton: false,
  isBreadCrumbsLoading: false,
  withSearch: true,
  withFooterInput: false,
  alwaysShowFooter: false,
  disableAcceptButton: false,
  withHeader: true,

  selectedItems: [],
};

export default Selector;
