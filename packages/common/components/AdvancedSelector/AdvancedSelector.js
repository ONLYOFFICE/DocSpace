import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";

import { isMobile } from "react-device-detect";
import { tablet } from "@docspace/components/utils/device";
import { Base } from "@docspace/components/themes";

import Selector from "./sub-components/Selector";
import Backdrop from "@docspace/components/backdrop";

import CrossIcon from "@docspace/components/public/static/images/cross.react.svg";

const StyledBlock = styled.div`
  position: fixed;
  top: 0;
  right: 0;

  width: 480px;
  max-width: 100%;
  height: 100%;

  z-index: 400;

  display: flex;
  flex-direction: column;

  background: ${(props) => props.theme.filterInput.filter.background};

  @media ${tablet} {
    max-width: calc(100% - 69px);
  }

  ${isMobile &&
  css`
    max-width: calc(100% - 69px);
  `}

  @media (max-width: 428px) {
    bottom: 0;
    top: unset;
    height: calc(100% - 64px);
    width: 100%;
    max-width: 100%;
  }

  .people-selector {
    height: 100%;
    width: 100%;

    .selector-wrapper,
    .column-options {
      width: 100%;
    }
  }
`;

StyledBlock.defaultProps = { theme: Base };

const StyledControlContainer = styled.div`
  display: flex;

  width: 24px;
  height: 24px;
  position: absolute;

  border-radius: 100px;
  cursor: pointer;

  align-items: center;
  justify-content: center;
  z-index: 450;

  top: 14px;
  left: -34px;

  ${isMobile &&
  css`
    top: 14px;
  `}

  @media (max-width: 428px) {
    top: -34px;
    right: 10px;
    left: unset;
  }
`;

StyledControlContainer.defaultProps = { theme: Base };

const StyledCrossIcon = styled(CrossIcon)`
  width: 17px;
  height: 17px;
  z-index: 455;
  path {
    fill: ${(props) => props.theme.catalog.control.fill};
  }
`;

StyledCrossIcon.defaultProps = { theme: Base };

class AdvancedSelector extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();
  }

  onClose = (e) => {
    //console.log("onClose");
    //this.setState({ isOpen: false });
    this.props.onCancel && this.props.onCancel(e);
  };

  render() {
    const { isOpen, id, className, style, withoutAside } = this.props;

    return (
      <>
        {isOpen && (
          <div id={id} className={className} style={style}>
            {withoutAside ? (
              <Selector {...this.props} />
            ) : (
              <>
                <Backdrop
                  onClick={this.onClose}
                  visible={isOpen}
                  zIndex={310}
                  isAside={true}
                />
                <StyledBlock>
                  <Selector {...this.props} />

                  <StyledControlContainer onClick={this.onClose}>
                    <StyledCrossIcon />
                  </StyledControlContainer>
                </StyledBlock>
              </>
            )}
          </div>
        )}
      </>
    );
  }
}

AdvancedSelector.propTypes = {
  id: PropTypes.string,
  className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  style: PropTypes.object,
  options: PropTypes.array,
  selectedOptions: PropTypes.array,
  groups: PropTypes.array,
  selectedGroups: PropTypes.array,

  value: PropTypes.string,
  placeholder: PropTypes.string,
  selectAllLabel: PropTypes.string,
  buttonLabel: PropTypes.string,

  maxHeight: PropTypes.number,

  isMultiSelect: PropTypes.bool,
  isDisabled: PropTypes.bool,
  selectedAll: PropTypes.bool,
  isOpen: PropTypes.bool,
  allowGroupSelection: PropTypes.bool,
  allowCreation: PropTypes.bool,
  allowAnyClickClose: PropTypes.bool,
  hasNextPage: PropTypes.bool,
  isNextPageLoading: PropTypes.bool,
  withoutAside: PropTypes.bool,

  onSearchChanged: PropTypes.func,
  onSelect: PropTypes.func,
  onGroupChange: PropTypes.func,
  onCancel: PropTypes.func,
  onAddNewClick: PropTypes.func,
  loadNextPage: PropTypes.func,
  isDefaultDisplayDropDown: PropTypes.bool,
};

AdvancedSelector.defaultProps = {
  isMultiSelect: false,
  size: "full",
  buttonLabel: "Add members",
  selectAllLabel: "Select all",
  allowGroupSelection: false,
  allowAnyClickClose: true,
  options: [],
  isDefaultDisplayDropDown: true,
};

export default AdvancedSelector;
