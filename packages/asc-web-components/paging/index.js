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
        className="buttonCustomStyle"
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
            scaledOptions={pageItems.length > 6}
            selectedOption={selectedPageItem}
            {...setDropDownMaxHeight}
          />
        </StyledPage>
      )}
      <Button
        className="buttonCustomStyle"
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
  previousLabel: PropTypes.string,
  nextLabel: PropTypes.string,

  previousAction: PropTypes.func,
  nextAction: PropTypes.func,

  disablePrevious: PropTypes.bool,
  disableNext: PropTypes.bool,
  disableHover: PropTypes.bool,

  selectedPageItem: PropTypes.object,
  selectedCountItem: PropTypes.object,

  onSelectPage: PropTypes.func,
  onSelectCount: PropTypes.func,

  pageItems: PropTypes.array,
  countItems: PropTypes.array,

  openDirection: PropTypes.oneOf(["bottom", "top"]),

  className: PropTypes.string,
  id: PropTypes.string,
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
