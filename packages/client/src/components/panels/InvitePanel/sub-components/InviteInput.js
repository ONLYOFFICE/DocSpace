import React, { useState, useCallback, useEffect, useRef } from "react";
import debounce from "lodash.debounce";
import { inject, observer } from "mobx-react";

import Avatar from "@docspace/components/avatar";
import TextInput from "@docspace/components/text-input";
import DropDownItem from "@docspace/components/drop-down-item";
import toastr from "@docspace/components/toast/toastr";
import { ShareAccessRights } from "@docspace/common/constants";
import { parseAddresses } from "@docspace/components/utils/email";

import { AddUsersPanel } from "../../index";
import { getAccessOptions } from "../utils";

import AccessSelector from "./AccessSelector";

import {
  StyledSubHeader,
  StyledLink,
  StyledInviteInput,
  StyledInviteInputContainer,
  StyledDropDown,
  SearchItemText,
  StyledDescription,
} from "../StyledInvitePanel";

const InviteInput = ({
  defaultAccess,
  getUsersByQuery,
  hideSelector,
  inviteItems,
  onClose,
  roomId,
  roomType,
  setInviteItems,
  roomUsers,
  t,
  isOwner,
  inputsRef,
}) => {
  const [inputValue, setInputValue] = useState("");
  const [usersList, setUsersList] = useState([]);
  const [searchPanelVisible, setSearchPanelVisible] = useState(false);
  const [addUsersPanelVisible, setAddUsersPanelVisible] = useState(false);

  const [selectedAccess, setSelectedAccess] = useState(defaultAccess);

  const searchRef = useRef();

  const inRoom = (id) => {
    return roomUsers.some((user) => user.sharedTo.id === id);
  };

  const toUserItems = (query) => {
    const addresses = parseAddresses(query);
    const uid = () => Math.random().toString(36).slice(-6);

    if (addresses.length > 1) {
      return addresses.map((address) => {
        return {
          email: address.email,
          id: uid(),
          access: selectedAccess,
          displayName: address.email,
          errors: address.parseErrors,
        };
      });
    }

    return {
      email: addresses[0].email,
      id: uid(),
      access: selectedAccess,
      displayName: addresses[0].email,
      errors: addresses[0].parseErrors,
    };
  };

  const searchByQuery = async (value) => {
    const query = value.trim();

    if (!!query.length) {
      const users = await getUsersByQuery(query);
      setUsersList(users);
    } else {
      closeInviteInputPanel();
      setInputValue("");
      setUsersList([]);
    }
  };

  const debouncedSearch = useCallback(
    debounce((value) => searchByQuery(value), 300),
    []
  );

  const onChange = (e) => {
    const value = e.target.value;
    const clearValue = value.trim();

    if ((!!usersList.length || clearValue.length > 2) && !searchPanelVisible) {
      openInviteInputPanel();
    }

    setInputValue(value);

    if (roomId !== -1) {
      debouncedSearch(clearValue);
    }
  };

  const removeExist = (items) => {
    const filtered = items.reduce((unique, o) => {
      !unique.some((obj) => obj.email === o.email) && unique.push(o);
      return unique;
    }, []);

    if (items.length > filtered.length)
      toastr.warning("Some users have already been added");

    return filtered;
  };

  const getItemContent = (item) => {
    const { avatar, displayName, email, id } = item;

    const invited = inRoom(id);

    item.access = selectedAccess;

    const addUser = () => {
      if (item.isOwner || item.isAdmin)
        item.access = ShareAccessRights.RoomManager;

      const items = removeExist([item, ...inviteItems]);

      setInviteItems(items);
      closeInviteInputPanel();
      setInputValue("");
      setUsersList([]);
    };

    return (
      <DropDownItem
        key={id}
        onClick={addUser}
        disabled={invited}
        height={48}
        className="list-item"
      >
        <Avatar size="min" role="user" source={avatar} />
        <div>
          <SearchItemText primary disabled={invited}>
            {displayName}
          </SearchItemText>
          <SearchItemText>{email}</SearchItemText>
        </div>
        {invited && <SearchItemText info>{t("Invited")}</SearchItemText>}
      </DropDownItem>
    );
  };

  const addEmail = () => {
    const items = toUserItems(inputValue);

    const newItems =
      items.length > 1 ? [...items, ...inviteItems] : [items, ...inviteItems];

    const filtered = removeExist(newItems);

    setInviteItems(filtered);
    closeInviteInputPanel();
    setInputValue("");
    setUsersList([]);
  };

  const addItems = (users) => {
    const items = [...users, ...inviteItems];

    const filtered = removeExist(items);

    setInviteItems(filtered);
    closeInviteInputPanel();
    setInputValue("");
    setUsersList([]);
  };

  const dropDownMaxHeight = usersList.length > 5 ? { maxHeight: 240 } : {};

  const openUsersPanel = () => {
    setInputValue("");
    setAddUsersPanelVisible(true);
  };

  const closeUsersPanel = () => {
    setAddUsersPanelVisible(false);
  };

  const openInviteInputPanel = (e) => {
    setSearchPanelVisible(true);
  };

  const closeInviteInputPanel = (e) => {
    // if (e?.target.tagName.toUpperCase() == "INPUT") return;

    setSearchPanelVisible(false);
  };

  const foundUsers = usersList.map((user) => getItemContent(user));

  const accessOptions = getAccessOptions(t, roomType);

  const onSelectAccess = (item) => {
    setSelectedAccess(item.access);
  };

  const onKeyPress = (e) => {
    if (e.key === "Enter" && !!!usersList.length && inputValue.length > 2) {
      addEmail();
    }
  };

  const onKeyDown = (event) => {
    const keyCode = event.code;

    const isAcceptableEvents =
      keyCode === "ArrowUp" || keyCode === "ArrowDown" || keyCode === "Enter";

    if (isAcceptableEvents && inputValue.length > 2) return;

    event.stopPropagation();
  };

  useEffect(() => {
    document.addEventListener("keyup", onKeyPress);
    return () => document.removeEventListener("keyup", onKeyPress);
  });

  return (
    <>
      <StyledSubHeader>
        {t("AddManually")}
        {!hideSelector && (
          <StyledLink
            className="link-list"
            fontWeight="600"
            type="action"
            isHovered
            onClick={openUsersPanel}
          >
            {t("Translations:ChooseFromList")}
          </StyledLink>
        )}
      </StyledSubHeader>
      <StyledDescription>
        {roomId === -1
          ? t("AddManuallyDescriptionAccounts")
          : t("AddManuallyDescriptionRoom")}
      </StyledDescription>

      <StyledInviteInputContainer ref={inputsRef}>
        <StyledInviteInput ref={searchRef}>
          <TextInput
            className="invite-input"
            scale
            onChange={onChange}
            placeholder={
              roomId === -1
                ? t("InviteAccountSearchPlaceholder")
                : t("InviteRoomSearchPlaceholder")
            }
            value={inputValue}
            onFocus={openInviteInputPanel}
            isAutoFocussed={true}
            onKeyDown={onKeyDown}
          />
        </StyledInviteInput>
        {inputValue.length > 2 && (
          <StyledDropDown
            width={searchRef?.current?.offsetWidth}
            isDefaultMode={false}
            open={searchPanelVisible}
            manualX="16px"
            showDisabledItems
            clickOutsideAction={closeInviteInputPanel}
            eventTypes="click"
            {...dropDownMaxHeight}
          >
            {!!usersList.length ? (
              foundUsers
            ) : (
              <DropDownItem
                className="add-item"
                style={{ width: "inherit" }}
                textOverflow
                onClick={addEmail}
                height={48}
              >
                {t("Common:AddButton")} «{inputValue}»
              </DropDownItem>
            )}
          </StyledDropDown>
        )}

        <AccessSelector
          className="add-manually-access"
          t={t}
          roomType={roomType}
          defaultAccess={selectedAccess}
          onSelectAccess={onSelectAccess}
          containerRef={inputsRef}
          isOwner={isOwner}
        />

        {!hideSelector && addUsersPanelVisible && (
          <AddUsersPanel
            onParentPanelClose={onClose}
            onClose={closeUsersPanel}
            visible={addUsersPanelVisible}
            shareDataItems={roomUsers}
            tempDataItems={inviteItems}
            setDataItems={addItems}
            accessOptions={accessOptions}
            isMultiSelect
            isEncrypted={true}
            defaultAccess={selectedAccess}
          />
        )}
      </StyledInviteInputContainer>
    </>
  );
};

export default inject(({ auth, peopleStore, filesStore, dialogsStore }) => {
  const { theme } = auth.settingsStore;
  const { isOwner } = auth.userStore.user;
  const { getUsersByQuery } = peopleStore.usersStore;
  const { invitePanelOptions, setInviteItems, inviteItems } = dialogsStore;

  return {
    setInviteItems,
    inviteItems,
    getUsersByQuery,
    roomId: invitePanelOptions.roomId,
    hideSelector: invitePanelOptions.hideSelector,
    defaultAccess: invitePanelOptions.defaultAccess,
    isOwner,
  };
})(observer(InviteInput));
