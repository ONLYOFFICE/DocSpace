import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";
import styled from "styled-components";
import Button from "../button";
import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";
import { mobile, tablet, hugeMobile } from "../utils/device";
import { Base } from "../themes";
import { isChrome, browserVersion } from "react-device-detect";

const StyledButton = styled(Button)`
  border: none;
  padding: 0 10px;
  height: 100%;
  min-width: fit-content;

  background-color: ${(props) => props.theme.button.backgroundColor.base};

  .combo-button_selected-icon {
    display: flex;
    align-items: center;
  }

  :hover {
    background-color: ${(props) =>
      props.theme.button.backgroundColor.baseHover};
  }
  :active {
    background-color: ${(props) =>
      props.theme.button.backgroundColor.baseActive};
  }

  svg {
    path[fill] {
      fill: ${(props) => props.theme.button.color.base};
    }

    path[stroke] {
      stroke: ${(props) => props.theme.button.color.base};
    }
  }

  :hover,
  :active {
    border: none;
    background-color: unset;
  }

  :hover {
    svg {
      path[fill] {
        fill: ${(props) => props.theme.button.color.baseHover};
      }

      path[stroke] {
        stroke: ${(props) => props.theme.button.color.baseHover};
      }
    }
  }

  :active {
    svg {
      path[fill] {
        fill: ${(props) => props.theme.button.color.baseActive};
      }

      path[stroke] {
        stroke: ${(props) => props.theme.button.color.baseActive};
      }
    }
  }

  .btnIcon {
    padding-right: 8px;
  }

  .button-content {
    @media ${tablet} {
      flex-direction: column;
      gap: 0px;
    }

    ${isChrome &&
    browserVersion <= 85 &&
    `
    /* TODO: remove if editors core version 85+ */
      > div {
        margin-right: 8px;

        @media ${tablet} {
          margin-right: 0px;
        }
      }
    `}
  }

  @media ${tablet} {
    display: flex;
    justify-content: center;
    flex-direction: column;
    padding: 0px 12px;
    .btnIcon {
      padding: 0;
      margin: 0 auto;
    }
  }

  @media ${mobile}, ${hugeMobile} {
    padding: 0 16px;
    font-size: 0;
    line-height: 0;
  }
`;

StyledButton.defaultProps = { theme: Base };

const GroupMenuItem = ({ item }) => {
  const buttonRef = React.useRef(null);

  const [open, setOpen] = React.useState(false);

  const onClickOutside = () => {
    setOpen(false);
  };

  const onClickAction = (e) => {
    onClick && onClick(e);

    if (withDropDown) {
      setOpen(true);
    }
  };

  const {
    label,
    disabled,
    onClick,
    iconUrl,
    title,
    withDropDown,
    options,
  } = item;
  return (
    <>
      {disabled ? (
        <></>
      ) : (
        <>
          <StyledButton
            label={label}
            title={title || label}
            isDisabled={disabled}
            onClick={onClickAction}
            icon={
              <ReactSVG src={iconUrl} className="combo-button_selected-icon" />
            }
            ref={buttonRef}
          />
          {withDropDown && (
            <DropDown
              open={open}
              clickOutsideAction={onClickOutside}
              forwardedRef={buttonRef}
              zIndex={250}
            >
              {options.map((option) => (
                <DropDownItem {...option} />
              ))}
            </DropDown>
          )}
        </>
      )}
    </>
  );
};

GroupMenuItem.propTypes = {
  item: PropTypes.object,
};

export default GroupMenuItem;
