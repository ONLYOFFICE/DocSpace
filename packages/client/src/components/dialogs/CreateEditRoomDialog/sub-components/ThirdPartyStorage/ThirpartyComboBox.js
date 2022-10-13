import React, { useEffect, useState, useRef } from "react";
import styled from "styled-components";
import { ReactSVG } from "react-svg";

import { StyledDropDown, StyledDropDownWrapper } from "../StyledDropdown";

import { isHugeMobile } from "@docspace/components/utils/device";
import DomHelpers from "@docspace/components/utils/domHelpers";

import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import DropDownItem from "@docspace/components/drop-down-item";
import { connectedCloudsTypeTitleTranslation as ProviderKeyTranslation } from "@docspace/client/src/helpers/filesUtils";
import { Base } from "@docspace/components/themes";
import { toastr } from "@docspace/components";

const StyledStorageLocation = styled.div`
  display: flex;
  flex-direction: column;

  .set_room_params-thirdparty {
    display: flex;
    flex-direction: row;
    gap: 8px;
    &-combobox {
      cursor: pointer;
      width: 100%;
      display: flex;
      flex-direction: row;
      justify-content: space-between;
      padding: 5px 7px;
      background: ${(props) =>
        props.theme.createEditRoomDialog.thirdpartyStorage.combobox.background};
      border-radius: 3px;
      max-height: 32px;

      border: ${(props) =>
        `1px solid ${
          props.isOpen
            ? props.theme.createEditRoomDialog.thirdpartyStorage.combobox
                .isOpenDropdownBorderColor
            : props.theme.createEditRoomDialog.thirdpartyStorage.combobox
                .dropdownBorderColor
        }`};

      transition: all 0.2s ease;
      &:hover {
        border: ${(props) =>
          `1px solid ${
            props.isOpen
              ? props.theme.createEditRoomDialog.thirdpartyStorage.combobox
                  .isOpenDropdownBorderColor
              : props.theme.createEditRoomDialog.thirdpartyStorage.combobox
                  .hoverDropdownBorderColor
          }`};
      }

      &-text {
        font-weight: 400;
        font-size: 13px;
        line-height: 20px;
      }

      &-expander {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 6.35px;
        svg {
          transform: ${(props) =>
            props.isOpen ? "rotate(180deg)" : "rotate(0)"};
          width: 6.35px;
          height: auto;
          path {
            fill: ${(props) =>
              props.theme.createEditRoomDialog.thirdpartyStorage.combobox
                .arrowFill};
          }
        }
      }
    }

    &-checkbox {
      margin-top: 8px;
      .checkbox {
        margin-right: 8px;
      }
      .checkbox-text {
        font-weight: 400;
        font-size: 13px;
        line-height: 20px;
      }
    }
  }
`;

StyledStorageLocation.defaultProps = { theme: Base };

const ThirpartyComboBox = ({
  t,

  storageLocation,
  onChangeProvider,
  onChangeThirdpartyAccount,

  connectItems,
  setConnectDialogVisible,
  setRoomCreation,
  saveThirdParty,

  saveThirdpartyResponse,
  setSaveThirdpartyResponse,
  openConnectWindow,
  setConnectItem,
  getOAuthToken,

  setIsScrollLocked,
  setIsOauthWindowOpen,
}) => {
  const dropdownRef = useRef(null);

  const thirdparties = connectItems.map((item) => ({
    ...item,
    title: item.category
      ? item.category
      : ProviderKeyTranslation(item.providerKey, t),
  }));

  const [isOpen, setIsOpen] = useState(false);
  const [dropdownDirection, setDropdownDirection] = useState("bottom");

  const toggleIsOpen = () => {
    if (isOpen) setIsScrollLocked(false);
    else {
      setIsScrollLocked(true);
      calculateDropdownDirection();
    }
    setIsOpen(!isOpen);
  };

  const setStorageLocaiton = (thirparty) => {
    onChangeProvider(thirparty);
    setIsOpen(false);
    setIsScrollLocked(false);
  };

  const calculateDropdownDirection = () => {
    const { top: offsetTop } = DomHelpers.getOffset(dropdownRef.current);
    const offsetBottom = window.innerHeight - offsetTop;

    const neededHeightDesktop = Math.min(thirdparties.length * 32 + 16, 404);
    const neededHeightMobile = Math.min(thirdparties.length * 32 + 16, 180);
    const neededheight = isHugeMobile()
      ? neededHeightMobile
      : neededHeightDesktop;

    setDropdownDirection(neededheight > offsetBottom ? "top" : "bottom");
  };

  const onShowService = async () => {
    setRoomCreation(true);
    const provider = storageLocation.provider;

    if (storageLocation.provider.isOauth) {
      setIsOauthWindowOpen(true);
      const authModal = window.open(
        "",
        "Authorization",
        "height=600, width=1020"
      );
      openConnectWindow(provider.providerKey, authModal).then((modal) =>
        getOAuthToken(modal)
          .then((token) =>
            saveThirdParty(
              provider.oauthHref,
              "",
              "",
              token,
              false,
              "ThirdpartyRoom",
              provider.providerKey,
              null,
              true
            ).then((res) => setSaveThirdpartyResponse(res))
          )
          .catch((e) => {
            if (!e) return;
            toastr.error(e);
            console.error(e);
          })
          .finally(() => {
            authModal.close();
            setIsOauthWindowOpen(false);
          })
      );
    } else {
      setConnectItem({
        title: ProviderKeyTranslation(provider.providerKey, t),
        customer_title: "ThirdpartyRoom",
        provider_key: provider.providerKey,
      });
      setConnectDialogVisible(true);
    }
  };

  useEffect(() => {
    if (!saveThirdpartyResponse?.id) return;

    console.log(saveThirdpartyResponse);
    onChangeThirdpartyAccount(saveThirdpartyResponse);
    setSaveThirdpartyResponse(null);
  }, [saveThirdpartyResponse]);

  return (
    <StyledStorageLocation isOpen={isOpen}>
      <div className="set_room_params-thirdparty">
        <div
          className="set_room_params-thirdparty-combobox"
          onClick={toggleIsOpen}
        >
          <Text className="set_room_params-thirdparty-combobox-text" noSelect>
            {storageLocation?.provider?.title ||
              t("ThirdPartyStorageComboBoxPlaceholder")}
          </Text>
          <ReactSVG
            className="set_room_params-thirdparty-combobox-expander"
            src={"/static/images/expander-down.react.svg"}
          />
        </div>

        <Button
          isDisabled={
            !storageLocation?.provider || !!storageLocation?.thirdpartyAccount
          }
          className="set_room_params-thirdparty-connect"
          size="small"
          label={t("Common:Connect")}
          onClick={onShowService}
        />
      </div>

      <StyledDropDownWrapper
        className="dropdown-content-wrapper"
        ref={dropdownRef}
      >
        <StyledDropDown
          className="dropdown-content"
          open={isOpen}
          forwardedRef={dropdownRef}
          clickOutsideAction={toggleIsOpen}
          maxHeight={isHugeMobile() ? 158 : 382}
          directionY={dropdownDirection}
          marginTop={dropdownDirection === "bottom" ? "4px" : "-36px"}
        >
          {thirdparties.map((thirdparty) => (
            <DropDownItem
              className="dropdown-item"
              label={thirdparty.title}
              key={thirdparty.id}
              height={32}
              heightTablet={32}
              onClick={() => setStorageLocaiton(thirdparty)}
            />
          ))}
        </StyledDropDown>
      </StyledDropDownWrapper>
    </StyledStorageLocation>
  );
};

export default ThirpartyComboBox;
