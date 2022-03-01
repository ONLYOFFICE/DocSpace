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
        className="buttonCustomStyle not-selectable"
        size="medium"
        scale={true}
        label={previousLabel}
        onClick={previousAction}
        isDisabled={disablePrevious}
        disableHover={disableHover}
      />
      {pageItems && (
        <StyledPage>
          <ComboBox
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
        className="buttonCustomStyle not-selectable"
        size="medium"
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
  /** Label for previous button */
  previousLabel: PropTypes.string,
  /** Label for next button */
  nextLabel: PropTypes.string,
  /** Action for previous button */
  previousAction: PropTypes.func,
  /** Action for next button */
  nextAction: PropTypes.func,
  /** Set previous button disabled */
  disablePrevious: PropTypes.bool,
  /** Set next button disabled */
  disableNext: PropTypes.bool,
  disableHover: PropTypes.bool,
  /** Initial value for pageItems */
  selectedPageItem: PropTypes.object,
  /** Initial value for countItems */
  selectedCountItem: PropTypes.object,

  onSelectPage: PropTypes.func,
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

  showCountItem: PropTypes.bool.isRequired,
};

Paging.defaultProps = {
  disablePrevious: false,
  disableNext: false,
  disableHover: false,
  showCountItem: true,
};

export default Paging;
