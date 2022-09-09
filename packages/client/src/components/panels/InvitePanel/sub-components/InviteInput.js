import React, { useState, useCallback, useEffect } from "react";
import debounce from "lodash.debounce";
import { inject, observer } from "mobx-react";

import Avatar from "@docspace/components/avatar";
import DropDownItem from "@docspace/components/drop-down-item";

import { parseAddresses } from "@docspace/components/utils/email";
import { ShareAccessRights } from "@docspace/common/constants";

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
} from "../StyledInvitePanel";

const InviteInput = ({
  invitePanelOptions,
  getShareUsers,
  setInviteItems,
  inviteItems,
  getUsersByQuery,
  t,
  onClose,
}) => {
  const [inputValue, setInputValue] = useState("");
  const [usersList, setUsersList] = useState([]);
  const [panelVisible, setPanelVisible] = useState(false);
  const [roomUsers, setRoomUsers] = useState([]);
  const [showUsersPanel, setShowUsersPanel] = useState(false);

  useEffect(() => {
    const { id } = invitePanelOptions;

    getShareUsers([id]).then((users) => setRoomUsers(users));
  }, [invitePanelOptions]);

  const inRoom = (id) => {
    return roomUsers.some((user) => user.sharedTo.id === id);
  };

  const toUserItem = (email) => {
    const emails = parseAddresses(email);
    const uid = () => Math.random().toString(36).slice(-6);

    return {
      email,
      id: uid(),
      displayName: email,
      errors: emails[0].parseErrors,
    };
  };

  const searchByQuery = async (value) => {
    const query = value.trim();

    if (!!query.length) {
      const users = await getUsersByQuery(query);
      setUsersList(users);
    }
  };

  const debouncedSearch = useCallback(
    debounce((value) => searchByQuery(value), 300),
    []
  );

  const onChange = (e) => {
    const value = e.target.value;
    const clearValue = value.trim();

    if ((!!usersList.length || clearValue.length > 1) && !panelVisible) {
      openInviteInputPanel();
    }

    setInputValue(value);
    debouncedSearch(clearValue);
  };

  const getItemContent = (item) => {
    const { avatarSmall, displayName, email, id } = item;

    const invited = inRoom(id);

    item.access = ShareAccessRights.ReadOnly; //TODO: get from main selector;

    const addUser = () => {
      setInviteItems([...inviteItems, item]);
      closeInviteInputPanel();
    };

    return (
      <DropDownItem
        key={id}
        onClick={addUser}
        disabled={invited}
        height={48}
        className="list-item"
      >
        <Avatar size="min" role="user" source={avatarSmall} />
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

  const addItem = () => {
    const item = toUserItem(inputValue);

    item.access = ShareAccessRights.ReadOnly; //TODO: get from main selector;

    setInviteItems([...inviteItems, item]);
    closeInviteInputPanel();
  };

  const addItems = (users) => {
    const items = [...inviteItems, ...users];

    const filtered = items.reduce((unique, o) => {
      !unique.some((obj) => obj.email === o.email) && unique.push(o);
      return unique;
    }, []);

    setInviteItems(filtered);
    closeInviteInputPanel();
  };

  const dropDownMaxHeight = usersList.length > 5 ? { maxHeight: 240 } : {};

  const openUsersPanel = () => {
    setInputValue("");
    setShowUsersPanel(true);
  };

  const closeUsersPanel = () => {
    setShowUsersPanel(false);
  };

  const openInviteInputPanel = () => {
    setPanelVisible(true);
  };

  const closeInviteInputPanel = () => {
    setInputValue("");
    setPanelVisible(false);
  };

  const foundUsers = usersList.map((user) => getItemContent(user));

  const accessOptions = getAccessOptions(t, 5);

  return (
    <>
      <StyledSubHeader>
        {t("IndividualInvitation")}
        <StyledLink
          fontWeight="600"
          type="action"
          isHovered
          onClick={openUsersPanel}
        >
          {t("СhooseFromList")}
        </StyledLink>
      </StyledSubHeader>

      <StyledInviteInputContainer>
        <StyledInviteInput
          onChange={onChange}
          placeholder={t("SearchPlaceholder")}
          value={inputValue}
        />
        <AccessSelector t={t} roomType={5} />
        <StyledDropDown
          isDefaultMode={false}
          open={panelVisible}
          manualX="16px"
          showDisabledItems
          clickOutsideAction={closeInviteInputPanel}
          {...dropDownMaxHeight}
        >
          {!!usersList.length
            ? foundUsers
            : inputValue.length > 2 && (
                <DropDownItem onClick={addItem} height={48}>
                  {t("Add")} «{inputValue}»
                </DropDownItem>
              )}
        </StyledDropDown>

        {showUsersPanel && (
          <AddUsersPanel
            onParentPanelClose={onClose}
            onClose={closeUsersPanel}
            visible={showUsersPanel}
            shareDataItems={roomUsers}
            tempDataItems={inviteItems}
            setDataItems={addItems}
            accessOptions={accessOptions}
            isMultiSelect
            isEncrypted={true}
          />
        )}
      </StyledInviteInputContainer>
    </>
  );
};

export default inject(({ auth, peopleStore, filesStore, dialogsStore }) => {
  const { theme } = auth.settingsStore;
  const { getUsersByQuery } = peopleStore.usersStore;
  const { getShareUsers } = filesStore;
  const { invitePanelOptions, setInviteItems, inviteItems } = dialogsStore;

  return {
    getShareUsers,
    setInviteItems,
    inviteItems,
    getUsersByQuery,
    invitePanelOptions,
  };
})(observer(InviteInput));
