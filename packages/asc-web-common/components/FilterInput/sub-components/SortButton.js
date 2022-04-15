import React from "react";
import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";
import { withTranslation } from "react-i18next";

import ComboBox from "@appserver/components/combobox";
import DropDownItem from "@appserver/components/drop-down-item";
import IconButton from "@appserver/components/icon-button";
import ViewSelector from "@appserver/components/view-selector";
import Text from "@appserver/components/text";

import { mobile } from "@appserver/components/utils/device";
import { Base } from "@appserver/components/themes";

import SortDesc from "../../../../../public/images/sort.desc.react.svg";

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

const mobileView = css`
  position: fixed;
  top: auto;
  left: 0;
  bottom: 0;
  width: 100vw;

  z-index: 999;
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

      @media ${mobile} {
        ${mobileView}
      }

      ${isMobileOnly && mobileView}

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

        min-width: 200px;

        svg {
          width: 16px;
          height: 16px;
        }

        .option-item__icon {
          display: none;
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
            display: flex;
          }
        }
      }

      .selected-option-item {
        background: ${(props) => props.theme.filterInput.sort.hoverBackground};
        cursor: auto;

        .selected-option-item__icon {
          display: flex;
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
  t,
  selectedFilterData,
  getSortData,
  onChangeViewAs,
  viewAs,
  viewSettings,
  onSort,
  viewSelectorVisible,
  isRecentFolder,
  isFavoritesFolder,
}) => {
  const [isOpen, setIsOpen] = React.useState(false);

  const [
    currentSelectedFilterData,
    setCurrentSelectedFilterData,
  ] = React.useState({});

  React.useEffect(() => {
    setCurrentSelectedFilterData({
      sortDirection: selectedFilterData.sortDirection,
      sortId: selectedFilterData.sortId,
    });
  }, [selectedFilterData]);

  const toggleCombobox = React.useCallback(() => {
    setIsOpen((val) => !val);
  }, []);

  const onOptionClick = React.useCallback(
    (e) => {
      const sortDirection =
        currentSelectedFilterData.sortDirection === "desc" &&
        e.target.closest(".option-item__icon")
          ? "asc"
          : "desc";

      const key = e.target.closest(".option-item").dataset.value;

      setCurrentSelectedFilterData({
        sortId: key,
        sortDirection: sortDirection,
      });

      toggleCombobox();
      onSort && onSort(key, sortDirection);
    },
    [onSort, toggleCombobox, currentSelectedFilterData]
  );

  const getSortOption = () => {};

  const getAdvancedOptions = React.useCallback(() => {
    const data = getSortData();

    data.forEach((item) => {
      item.className = "option-item";
      item.isSelected = false;
      if (currentSelectedFilterData.sortId === item.key) {
        item.className = item.className + " selected-option-item";
        item.isSelected = true;
      }
    });

    return (
      <>
        {viewSelectorVisible && (
          <>
            <DropDownItem noHover className="view-selector-item">
              <Text fontWeight={600}>{t("View")}</Text>
              <ViewSelector
                className="view-selector"
                onChangeView={onChangeViewAs}
                viewAs={viewAs}
                viewSettings={viewSettings}
              />
            </DropDownItem>
            {!isFavoritesFolder && !isRecentFolder && (
              <DropDownItem isSeparator={true}></DropDownItem>
            )}
          </>
        )}
        {!isFavoritesFolder && !isRecentFolder && (
          <>
            {data.map((item, index) => (
              <DropDownItem
                onClick={onOptionClick}
                className={item.className}
                key={item.key}
                data-value={item.key}
              >
                <Text fontWeight={600}>{item.label}</Text>
                <SortDesc
                  className={`option-item__icon  ${
                    item.isSelected ? "selected-option-item__icon" : ""
                  }`}
                />
              </DropDownItem>
            ))}
          </>
        )}
      </>
    );
  }, [
    currentSelectedFilterData,
    onOptionClick,
    onChangeViewAs,
    viewAs,
    viewSettings,
    getSortData,
    isFavoritesFolder,
    isRecentFolder,
  ]);

  return (
    <StyledSortButton
      viewAs={viewAs}
      isDesc={currentSelectedFilterData.sortDirection === "desc"}
      onClick={toggleCombobox}
    >
      <ComboBox
        opened={isOpen}
        toggleAction={toggleCombobox}
        className={"sort-combo-box"}
        options={[]}
        selectedOption={{}}
        directionX={"right"}
        directionY={"both"}
        scaled={true}
        size={"content"}
        advancedOptions={getAdvancedOptions()}
        disableIconClick={false}
        disableItemClick={true}
        isDefaultMode={false}
        manualY={"102%"}
      >
        <IconButton iconName="/static/images/sort.react.svg" size={16} />
      </ComboBox>
    </StyledSortButton>
  );
};

export default React.memo(withTranslation("Common")(SortButton));
