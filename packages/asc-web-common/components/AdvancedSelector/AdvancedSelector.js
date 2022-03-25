import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";

import { isMobileOnly } from "react-device-detect";
import { mobile } from "@appserver/components/utils/device";
import { Base } from "@appserver/components/themes";

import Selector from "./sub-components/Selector";
import Backdrop from "@appserver/components/backdrop";

const mobileView = css`
  top: 64px;

  width: 100vw !important;
  height: calc(100vh - 64px) !important;
`;

const StyledBlock = styled.div`
  position: fixed;
  top: 0;
  right: 0;

  width: 480px;
  max-width: 100vw;
  height: 100vh;

  z-index: 400;

  display: flex;
  flex-direction: column;

  background: ${(props) => props.theme.filterInput.filter.background};

  @media ${mobile} {
    ${mobileView}
  }

  ${isMobileOnly && mobileView}

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
