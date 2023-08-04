import React from "react";
import PropTypes from "prop-types";

import Button from "../button";
import ComboBox from "../combobox";
import { StyledPage, StyledOnPage, StyledPaging } from "./styled-paging";

const Paging = (props) => {
  //console.log("Paging render");
  const {
    previousLabel,
    nextLabel,
    previousAction,
    nextAction,
    pageItems,
    countItems,
    openDirection,
    disablePrevious,
    disableNext,
    disableHover,
    selectedPageItem,
    selectedCountItem,
    id,
    className,
    style,
    showCountItem,
  } = props;

  const onSelectPageAction = (option) => {
    props.onSelectPage(option);
  };

  const onSelectCountAction = (option) => {
    props.onSelectCount(option);
  };

  const setDropDownMaxHeight =
    pageItems && pageItems.length > 6 ? { dropDownMaxHeight: 200 } : {};

  return (
    <StyledPaging id={id} className={className} style={style}>
      <Button
        className="not-selectable"
        size="small"
        scale={true}
        label={previousLabel}
        onClick={previousAction}
        isDisabled={disablePrevious}
        disableHover={disableHover}
      />
      {pageItems && (
        <StyledPage>
          <ComboBox
            isDisabled={disablePrevious && disableNext}
            className="manualWidth"
            directionY={openDirection}
            options={pageItems}
            onSelect={onSelectPageAction}
            scaledOptions={pageItems.length < 6}
            selectedOption={selectedPageItem}
            {...setDropDownMaxHeight}
          />
        </StyledPage>
      )}
      <Button
        className="not-selectable"
        size="small"
        scale={true}
        label={nextLabel}
        onClick={nextAction}
        isDisabled={disableNext}
        disableHover={disableHover}
      />
      {showCountItem
        ? countItems && (
            <StyledOnPage>
              <ComboBox
                className="hideDisabled"
                directionY={openDirection}
                directionX="right"
                options={countItems}
                scaledOptions={true}
                onSelect={onSelectCountAction}
                selectedOption={selectedCountItem}
              />
            </StyledOnPage>
          )
        : null}
    </StyledPaging>
  );
};

Paging.propTypes = {
  /** Label for the previous button */
  previousLabel: PropTypes.string,
  /** Label for the next button */
  nextLabel: PropTypes.string,
  /** Action for the previous button */
  previousAction: PropTypes.func,
  /** Action for the next button */
  nextAction: PropTypes.func,
  /** Sets previous button disabled */
  disablePrevious: PropTypes.bool,
  /** Sets the next button disabled */
  disableNext: PropTypes.bool,
  /** Disables the hover action for buttons */
  disableHover: PropTypes.bool,
  /** Initial value for pageItems */
  selectedPageItem: PropTypes.object,
  /** Initial value for countItems */
  selectedCountItem: PropTypes.object,
  /** Sets a callback function that is triggered when the page is selected */
  onSelectPage: PropTypes.func,
  /** Sets a callback function that is triggered when the page items are selected */
  onSelectCount: PropTypes.func,
  /** Paging combo box items */
  pageItems: PropTypes.array,
  /** Items per page combo box items */
  countItems: PropTypes.array,
  /** Indicates opening direction of combo box */
  openDirection: PropTypes.oneOf(["bottom", "top", "both"]),
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Displays a combobox with the number of items per page */
  showCountItem: PropTypes.bool.isRequired,
};

Paging.defaultProps = {
  disablePrevious: false,
  disableNext: false,
  disableHover: false,
  showCountItem: true,
};

export default Paging;
