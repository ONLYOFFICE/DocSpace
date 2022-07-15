import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import Heading from "@appserver/components/heading";
import { isMobile } from "react-device-detect";
import {
  tablet,
  desktop,
  isTablet,
  isMobile as isMobileUtils,
} from "@appserver/components/utils/device";
import DropDownItem from "@appserver/components/drop-down-item";
import Text from "@appserver/components/text";
import IconButton from "@appserver/components/icon-button";
import ComboBox from "@appserver/components/combobox";
import { Base } from "@appserver/components/themes";

import SortDesc from "../../../../../../../../../../public/images/sort.desc.react.svg";

const paddingCss = css`
  @media ${desktop} {
    margin-left: 1px;
    padding-right: 0px;
  }

  @media ${tablet} {
    margin-left: -1px;
  }
`;

const StyledGridWrapper = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(216px, 1fr));
  width: 100%;
  margin-bottom: ${(props) => (props.isFolders ? "23px" : 0)};
  box-sizing: border-box;
  ${paddingCss};

  grid-gap: 14px 16px;

  @media ${tablet} {
    grid-gap: 14px;
  }

  .tiles-loader {
    padding-top: 14px;

    &:nth-of-type(n + 3) {
      display: block;
    }
  }
`;

const StyledTileContainer = styled.div`
  position: relative;

  .tile-item-wrapper {
    position: relative;
    width: 100%;

    &.file {
      padding: 0;
    }
    &.folder {
      padding: 0;
    }
  }

  .tile-items-heading {
    margin: 0;
    margin-bottom: 15px;

    display: flex;
    align-items: center;
    justify-content: space-between;

    div {
      cursor: pointer !important;

      .sort-combo-box {
        margin-right: 3px;
        .dropdown-container {
          top: 104%;
          bottom: auto;
          min-width: 200px;
          margin-top: 3px;

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
            background: ${(props) =>
              props.theme.filterInput.sort.hoverBackground};
            cursor: auto;

            .selected-option-item__icon {
              display: flex;
            }
          }
        }

        .optionalBlock {
          display: flex;
          flex-direction: row;
          align-items: center;

          font-size: 12px;
          font-weight: 600;

          color: ${(props) => props.theme.filterInput.sort.tileSortColor};

          .sort-icon {
            margin-right: 8px;
            svg {
              path {
                fill: ${(props) => props.theme.filterInput.sort.tileSortFill};
              }
            }
          }
        }

        .combo-buttons_arrow-icon {
          display: none;
        }
      }
    }
  }

  @media ${tablet} {
    margin-right: -3px;
  }
`;

StyledTileContainer.defaultProps = { theme: Base };

class TileContainer extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      isOpen: false,
      selectedFilterData: {
        sortId: props.filter.sortBy,
        sortDirection: props.filter.sortOrder,
      },
    };
  }

  toggleDropdown = () => {
    this.setState((prev) => ({
      isOpen: !prev.isOpen,
    }));
  };

  getSortData = () => {
    const { t, personal } = this.props;

    const commonOptions = [
      { key: "AZ", label: t("ByTitle"), default: true },
      { key: "Type", label: t("Common:Type"), default: true },
      { key: "Size", label: t("Common:Size"), default: true },
      {
        key: "DateAndTimeCreation",
        label: t("ByCreationDate"),
        default: true,
      },
      { key: "DateAndTime", label: t("ByLastModifiedDate"), default: true },
    ];

    if (!personal) {
      commonOptions.splice(1, 0, {
        key: "Author",
        label: t("ByAuthor"),
        default: true,
      });
    }
    return commonOptions;
  };

  onSort = (sortId, sortDirection) => {
    const { filter, setIsLoading, fetchFiles, selectedFolderId } = this.props;

    const sortBy = sortId;
    const sortOrder = sortDirection === "desc" ? "descending" : "ascending";

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.sortBy = sortBy;
    newFilter.sortOrder = sortOrder;

    setIsLoading(true);

    fetchFiles(selectedFolderId, newFilter).finally(() => setIsLoading(false));
  };

  onOptionClick = (e) => {
    const key = e.target.closest(".option-item").dataset.value;

    let sortDirection = this.state.selectedFilterData.sortDirection;

    if (key === this.state.selectedFilterData.sortId) {
      sortDirection = sortDirection === "desc" ? "asc" : "desc";
    }

    this.setState({
      selectedFilterData: {
        sortId: key,
        sortDirection: sortDirection,
      },
    });

    this.toggleDropdown();
    this.onSort(key, sortDirection);
  };

  getAdvancedOptions = () => {
    const { filter } = this.props;

    const selectedFilterData = {
      sortDirection: filter.sortOrder === "ascending" ? "asc" : "desc",
      sortId: filter.sortBy,
    };

    const data = this.getSortData();

    data.forEach((item) => {
      item.className = "option-item";
      item.isSelected = false;
      if (selectedFilterData.sortId === item.key) {
        item.className = item.className + " selected-option-item";
        item.isSelected = true;
      }
    });

    return (
      <>
        {data.map((item, index) => (
          <DropDownItem
            onClick={this.onOptionClick}
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
    );
  };

  render() {
    const {
      t,
      children,
      useReactWindow,
      id,
      className,
      style,
      headingFolders,
      headingFiles,
      isRecentFolder,
      isFavoritesFolder,
    } = this.props;

    const { isOpen, selectedFilterData } = this.state;

    const Folders = [];
    const Files = [];

    React.Children.map(children, (item, index) => {
      const { isFolder, fileExst, id } = item.props.item;
      if ((isFolder || id === -1) && !fileExst) {
        Folders.push(
          <div className="tile-item-wrapper folder" key={index}>
            {item}
          </div>
        );
      } else {
        Files.push(
          <div className="tile-item-wrapper file" key={index}>
            {item}
          </div>
        );
      }
    });

    const advancedOptions = this.getAdvancedOptions();

    const renderSorting = () => {
      return (
        <>
          {!isRecentFolder &&
            !isFavoritesFolder &&
            !isMobile &&
            !isTablet() &&
            !isMobileUtils() && (
              <div onClick={this.toggleDropdown}>
                <ComboBox
                  opened={isOpen}
                  className={"sort-combo-box"}
                  options={[]}
                  selectedOption={{}}
                  directionX={"right"}
                  directionY={"both"}
                  scaled={false}
                  size={"content"}
                  advancedOptions={advancedOptions}
                  disableIconClick={false}
                  // disableItemClick={true}
                  isDefaultMode={false}
                  noBorder={true}
                  manualY={"102%"}
                >
                  <IconButton
                    className={"sort-icon"}
                    iconName="/static/images/sort.react.svg"
                    size={16}
                  />
                  {t("Common:Sorting")}
                </ComboBox>
              </div>
            )}
        </>
      );
    };

    return (
      <StyledTileContainer
        id={id}
        className={className}
        style={style}
        useReactWindow={useReactWindow}
        isDesc={selectedFilterData.sortDirection === "desc"}
      >
        {Folders.length > 0 && (
          <Heading
            size="xsmall"
            id={"folder-tile-heading"}
            className="tile-items-heading"
          >
            {headingFolders}
            {renderSorting()}
          </Heading>
        )}
        {Folders.length > 0 && (
          <StyledGridWrapper isFolders>{Folders}</StyledGridWrapper>
        )}

        {Files.length > 0 && (
          <Heading size="xsmall" className="tile-items-heading">
            {headingFiles}
            {Folders.length === 0 && renderSorting()}
          </Heading>
        )}
        {Files.length > 0 && <StyledGridWrapper>{Files}</StyledGridWrapper>}
      </StyledTileContainer>
    );
  }
}

TileContainer.propTypes = {
  children: PropTypes.any.isRequired,
  useReactWindow: PropTypes.bool,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

TileContainer.defaultProps = {
  useReactWindow: true,
  id: "tileContainer",
};

export default inject(
  ({ auth, filesStore, treeFoldersStore, selectedFolderStore }) => {
    const { personal } = auth.settingsStore;
    const { fetchFiles, filter, setIsLoading } = filesStore;
    const { isFavoritesFolder, isRecentFolder } = treeFoldersStore;

    return {
      personal,
      fetchFiles,
      filter,
      setIsLoading,
      isFavoritesFolder,
      isRecentFolder,
      selectedFolderId: selectedFolderStore.id,
    };
  }
)(observer(withTranslation(["Home", "Common"])(TileContainer)));
