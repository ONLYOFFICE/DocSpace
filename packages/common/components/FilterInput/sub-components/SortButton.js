import React from "react";
import styled, { css } from "styled-components";
import { withTranslation } from "react-i18next";

import ComboBox from "@docspace/components/combobox";
import DropDownItem from "@docspace/components/drop-down-item";
import IconButton from "@docspace/components/icon-button";
import ViewSelector from "@docspace/components/view-selector";
import Text from "@docspace/components/text";

import { mobile } from "@docspace/components/utils/device";
import { Base } from "@docspace/components/themes";

import SortDesc from "PUBLIC_DIR/images/sort.desc.react.svg";
import SortReactSvgUrl from "PUBLIC_DIR/images/sort.react.svg?url";
import Backdrop from "@docspace/components/backdrop";
import { Events } from "@docspace/common/constants";

const selectedViewIcon = css`
  svg {
    path {
      fill: ${(props) => props.theme.filterInput.sort.selectedViewIcon};
    }
  }
`;

const notSelectedViewIcon = css`
  svg {
    path {
      fill: ${(props) => props.theme.filterInput.sort.viewIcon};
    }
  }
`;

const StyledSortButton = styled.div`
  .combo-button {
    background: ${(props) =>
      props.theme.filterInput.sort.background} !important;

    .icon-button_svg {
      cursor: pointer;
    }
  }

  .sort-combo-box {
    width: 32px;
    height: 32px;

    margin-left: 8px;

    .dropdown-container {
      top: 102%;
      bottom: auto;
      min-width: 200px;
      margin-top: 3px;

      .view-selector-item {
        display: flex;
        align-items: center;
        justify-content: space-between;

        cursor: auto;

        .view-selector {
          width: 44px;

          display: flex;
          align-items: center;
          justify-content: space-between;

          cursor: auto;

          .view-selector-icon {
            border: none;
            background: transparent;
            padding: 0;

            div {
              display: flex;
              align-items: center;
              justify-content: center;
            }
          }

          .view-selector-icon:nth-child(1) {
            ${(props) =>
              props.viewAs === "row" ? selectedViewIcon : notSelectedViewIcon};
          }

          .view-selector-icon:nth-child(2) {
            ${(props) =>
              props.viewAs !== "row" ? selectedViewIcon : notSelectedViewIcon};
          }
        }
      }

      .option-item {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 12px;
        min-width: 200px;

        svg {
          width: 16px;
          height: 16px;
        }

        .option-item__icon {
          display: flex;
          visibility: hidden;
          cursor: pointer;
          ${(props) =>
            props.isDesc &&
            css`
              transform: rotate(180deg);
            `}

          path {
            fill: ${(props) => props.theme.filterInput.sort.sortFill};
          }
        }

        :hover {
          .option-item__icon {
            visibility: visible;
          }
        }
      }

      .selected-option-item {
        background: ${(props) => props.theme.filterInput.sort.hoverBackground};
        cursor: auto;

        .selected-option-item__icon {
          visibility: visible;
        }
      }
    }

    .optionalBlock {
      display: flex;
      align-items: center;
      justify-content: center;

      margin-right: 0;
    }

    .combo-buttons_arrow-icon {
      display: none;
    }

    .backdrop-active {
      display: none;
    }
  }
`;

StyledSortButton.defaultProps = { theme: Base };

const SortButton = ({
  id,
  getSortData,
  getSelectedSortData,

  onChangeViewAs,
  view,
  viewAs,
  viewSettings,

  onSort,
  viewSelectorVisible,

  onSortButtonClick,
  title,
}) => {
  const [isOpen, setIsOpen] = React.useState(false);

  const [sortData, setSortData] = React.useState([]);
  const [selectedSortData, setSelectedSortData] = React.useState({
    sortDirection: null,
    sortId: null,
  });

  const getSortDataAction = React.useCallback(() => {
    const value = getSortData && getSortData();
    const selectedValue = getSelectedSortData && getSelectedSortData();

    const data = value.map((item) => {
      item.className = "option-item";
      item.isSelected = false;
      if (selectedValue.sortId === item.key) {
        item.className = item.className + " selected-option-item";
        item.isSelected = true;
      }

      return item;
    });

    setSortData(data);

    setSelectedSortData({
      sortDirection: selectedValue.sortDirection,
      sortId: selectedValue.sortId,
    });
  }, [getSortData, getSelectedSortData, viewAs]);

  React.useEffect(() => {
    window.addEventListener(Events.CHANGE_COLUMN, getSortDataAction);
    getSortDataAction();

    return () =>
      window.removeEventListener(Events.CHANGE_COLUMN, getSortDataAction);
  }, [getSortDataAction]);

  const toggleCombobox = React.useCallback(() => {
    setIsOpen((val) => !val);
  }, []);

  React.useEffect(() => {
    onSortButtonClick && onSortButtonClick(!isOpen);
  }, [isOpen]);

  const onOptionClick = React.useCallback(
    (e) => {
      const key = e.target.closest(".option-item").dataset.value;

      let sortDirection = selectedSortData.sortDirection;

      if (key === selectedSortData.sortId) {
        sortDirection = sortDirection === "desc" ? "asc" : "desc";
      }

      let data = sortData.map((item) => ({ ...item }));

      data = data.map((item) => {
        item.className = "option-item";
        item.isSelected = false;
        if (key === item.key) {
          item.className = item.className + " selected-option-item";
          item.isSelected = true;
        }

        return item;
      });

      setSortData(data);

      setSelectedSortData({
        sortId: key,
        sortDirection: sortDirection,
      });

      toggleCombobox();

      onSort && onSort(key, sortDirection);
    },
    [onSort, toggleCombobox, sortData, selectedSortData]
  );

  const advancedOptions = (
    <>
      {viewSelectorVisible && (
        <>
          <DropDownItem noHover className="view-selector-item">
            <Text fontWeight={600}>{view}</Text>
            <ViewSelector
              className="view-selector"
              onChangeView={onChangeViewAs}
              viewAs={viewAs}
              viewSettings={viewSettings}
            />
          </DropDownItem>

          <DropDownItem isSeparator={true}></DropDownItem>
        </>
      )}
      {sortData?.map((item) => (
        <DropDownItem
          id={item.id}
          onClick={onOptionClick}
          className={item.className}
          key={item.key}
          data-value={item.key}
        >
          <Text fontWeight={600}>{item.label}</Text>
          <SortDesc
            className={`option-item__icon${
              item.isSelected ? " selected-option-item__icon" : ""
            }`}
          />
        </DropDownItem>
      ))}
    </>
  );

  let advancedOptionsCount = sortData.length;

  if (viewSelectorVisible) {
    advancedOptionsCount++;
  }

  return (
    <>
      <Backdrop
        visible={isOpen}
        withBackground={false}
        onClick={toggleCombobox}
        withoutBlur={true}
      />
      <StyledSortButton
        viewAs={viewAs}
        isDesc={selectedSortData.sortDirection === "desc"}
        onClick={toggleCombobox}
        id={id}
        title={title}
      >
        <ComboBox
          opened={isOpen}
          onToggle={toggleCombobox}
          className={"sort-combo-box"}
          options={[]}
          selectedOption={{}}
          directionX={"right"}
          directionY={"both"}
          scaled={true}
          size={"content"}
          advancedOptions={advancedOptions}
          disableIconClick={false}
          disableItemClick={true}
          isDefaultMode={false}
          manualY={"102%"}
          advancedOptionsCount={advancedOptionsCount}
        >
          <IconButton iconName={SortReactSvgUrl} size={16} />
        </ComboBox>
      </StyledSortButton>
    </>
  );
};

export default React.memo(SortButton);
