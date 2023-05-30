import ExpanderDownReactSvgUrl from "PUBLIC_DIR/images/expander-down.react.svg?url";
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

const ThirdPartyComboBox = ({
  t,

  storageLocation,
  onChangeStorageLocation,
  onChangeProvider,

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

  isDisabled,
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
    if (isDisabled) return;
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
        t("Common:Authorization"),
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
              ProviderKeyTranslation(provider.providerKey, t),
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
      const providerTitle = ProviderKeyTranslation(provider.providerKey, t);
      setConnectItem({
        title: providerTitle,
        customer_title: providerTitle,
        provider_key: provider.providerKey,
      });
      setConnectDialogVisible(true);
    }
  };

  useEffect(() => {
    if (!saveThirdpartyResponse?.id) return;
    onChangeStorageLocation({
      ...storageLocation,
      thirdpartyAccount: saveThirdpartyResponse,
      storageFolderId: saveThirdpartyResponse.id,
    });
    setSaveThirdpartyResponse(null);
  }, [saveThirdpartyResponse]);

  return (
    <StyledStorageLocation isOpen={isOpen}>
      <div className="set_room_params-thirdparty">
        <div
          id="shared_third-party-storage_combobox"
          className="set_room_params-thirdparty-combobox"
          onClick={toggleIsOpen}
        >
          <Text className="set_room_params-thirdparty-combobox-text" noSelect>
            {storageLocation?.provider?.title ||
              t("ThirdPartyStorageComboBoxPlaceholder")}
          </Text>
          <ReactSVG
            className="set_room_params-thirdparty-combobox-expander"
            src={ExpanderDownReactSvgUrl}
          />
        </div>

        <Button
          id="shared_third-party-storage_connect"
          isDisabled={
            !storageLocation?.provider ||
            !!storageLocation?.thirdpartyAccount ||
            isDisabled
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
          hasItems={isOpen}
        >
          {thirdparties.map((thirdparty) => (
            <DropDownItem
              id={thirdparty.id}
              className={`dropdown-item ${thirdparty.className ?? ""}`}
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

export default ThirdPartyComboBox;
