import Button from "@appserver/components/button";
import ComboBox from "@appserver/components/combobox";
import FieldContainer from "@appserver/components/field-container";
import SearchInput from "@appserver/components/search-input";
import SelectedItem from "@appserver/components/selected-item";
import TextInput from "@appserver/components/text-input";
import { tablet } from "@appserver/components/utils/device";

import toastr from "studio/toastr";
import Loaders from "@appserver/common/components/Loaders";
//import PeopleSelector from "people/PeopleSelector";
import PeopleSelector from "../../../../components/PeopleSelector";
import { GUID_EMPTY } from "../../../../helpers/constants";
import PropTypes from "prop-types";
import React, { useEffect, useState } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import CatalogGuestIcon from "../../../../../public/images/catalog.guest.react.svg";
import { ID_NO_GROUP_MANAGER } from "../../../../helpers/constants";
import { inject, observer } from "mobx-react";
import config from "../../../../../package.json";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import { useTranslation } from "react-i18next";

import withLoader from "../../../../HOCs/withLoader";

const MainContainer = styled.div`
  display: flex;
  flex-direction: column;
  max-width: 1024px;

  .group-name_container {
    max-width: 320px;
  }

  .head_container {
    position: relative;
    max-width: 320px;
  }

  .members_container {
    position: relative;
    max-width: 320px;
    margin: 0;
  }

  .search_container {
    margin-top: 32px;
    display: none;
  }

  .selected-members_container {
    margin-top: 32px;
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    grid-row-gap: 8px;
    grid-column-gap: 16px;
  }

  .buttons_container {
    margin-top: 40px;

    .cancel-button {
      margin-left: 8px;
    }
  }

  @media ${tablet} {
    max-width: 320px;

    .selected-members_container {
      grid-template-columns: repeat(1, 1fr);
    }
  }
`;

const SectionBodyContent = ({
  id,
  name,
  groupManager,
  groupMembers,
  me,
  groups,
  createGroup,
  updateGroup,
  selectGroup,
  groupCaption,
  groupsCaption,
  groupHeadCaption,
  history,
  resetGroup,
  filter,
  setFilter,
  isLoaded,
  tReady,
}) => {
  const { t, i18n } = useTranslation(["GroupAction", "Translations", "Common"]);

  const [inLoading, setInLoading] = useState(false);
  const [isHeadSelectorOpen, setIsHeadSelectorOpen] = useState(false);
  const [isUsersSelectorOpen, setIsUsersSelectorOpen] = useState(false);

  const [groupName, setGroupName] = useState(name || "");
  const [newGroupManager, setNewGroupManager] = useState(groupManager);
  const [newGroupMembers, setNewGroupMembers] = useState(groupMembers);

  const [searchValue, setSearchValue] = useState("");
  const [nameError, setNameError] = useState("");

  const onGroupChange = (e) => {
    setGroupName(e.target.value);
  };

  const onSearchChange = (value) => {
    setSearchValue(value);
  };

  const onGroupManagerSelect = (options) => {
    if (!options || !options.length) return;

    const option = options[0];

    setNewGroupManager({
      key: option.key,
      label: option.label,
    });

    setIsHeadSelectorOpen(!isHeadSelectorOpen);
  };

  const onHeadSelectorClick = () => {
    setIsHeadSelectorOpen(!isHeadSelectorOpen);
  };

  const onUsersSelectorSelect = (selectedOptions) => {
    setNewGroupMembers(
      selectedOptions.map((option) => {
        return {
          key: option.key,
          label: option.label,
        };
      })
    );

    onUsersSelectorClick();
  };

  const onUsersSelectorClick = () => {
    setIsUsersSelectorOpen(!isUsersSelectorOpen);
  };

  const save = (group) => {
    return id
      ? updateGroup(group.id, group.name, group.managerKey, group.members)
      : createGroup(group.name, group.managerKey, group.members);
  };

  const onSave = () => {
    if (!groupName || !groupName.trim().length) {
      setNameError(t("Common:EmptyFieldError"));
      return false;
    }

    setInLoading(true);

    const newGroup = {
      name: groupName.trim(),
      managerKey: newGroupManager.key,
      members: newGroupMembers.map((u) => u.key),
    };

    if (id) newGroup.id = id;

    save(newGroup)
      .then((group) => {
        toastr.success(
          t("SuccessSaveGroup", { groupCaption, groupName: group.name })
        );

        if (id) {
          history.goBack();
          return selectGroup(group.id);
        } else {
          return history.push(
            combineUrl(AppServerConfig.proxyURL, config.homepage, "/")
          );
        }
      })
      .catch((error) => {
        toastr.error(error);
        setInLoading(false);
      });
  };

  const onCancel = () => {
    resetGroup();
    history.goBack();
    setFilter(filter);
  };

  const onSelectedItemClose = (member) => {
    setNewGroupMembers(newGroupMembers.filter((g) => g.key !== member.key));
  };

  const onCancelSelector = (e) => {
    if (
      (isHeadSelectorOpen &&
        (e.target.id === "head-selector_button" ||
          e.target.closest("#head-selector_button"))) ||
      (isUsersSelectorOpen &&
        (e.target.id === "users-selector_button" ||
          e.target.closest("#users-selector_button")))
    ) {
      // Skip double set of isOpen property
      return;
    }

    setIsHeadSelectorOpen(false);
    setIsUsersSelectorOpen(false);
  };

  const onKeyPress = (event) => {
    if (event.key === "Enter") {
      onSave();
    }
  };

  const onFocusName = () => {
    if (nameError) setNameError(null);
  };

  const buttonLabel = id ? t("Common:SaveButton") : t("Common:AddButton");

  return (
    <MainContainer>
      <>
        <FieldContainer
          className="group-name_container"
          isRequired={true}
          hasError={!!nameError}
          errorMessage={nameError}
          isVertical={true}
          labelText={t("Common:Name")}
        >
          <TextInput
            id="group-name"
            name="group-name"
            scale={true}
            isAutoFocussed={true}
            isBold={true}
            tabIndex={1}
            value={groupName}
            hasError={!!nameError}
            onChange={onGroupChange}
            isDisabled={inLoading}
            onKeyUp={onKeyPress}
            onFocus={onFocusName}
          />
        </FieldContainer>
        <FieldContainer
          className="head_container"
          isRequired={false}
          hasError={false}
          isVertical={true}
          labelText={groupHeadCaption}
        >
          <ComboBox
            id="head-selector_button"
            tabIndex={2}
            options={[]}
            opened={isHeadSelectorOpen}
            selectedOption={
              newGroupManager.default ||
              newGroupManager.key === ID_NO_GROUP_MANAGER ||
              newGroupManager.displayName === "profile removed"
                ? { ...newGroupManager, label: t("SelectAction") }
                : newGroupManager
            }
            scaled={true}
            isDisabled={inLoading}
            size="content"
            toggleAction={onHeadSelectorClick}
            displayType="toggle"
          >
            <CatalogGuestIcon size="medium" />
          </ComboBox>
          <PeopleSelector
            isOpen={isHeadSelectorOpen}
            onSelect={onGroupManagerSelect}
            onCancel={onCancelSelector}
            groupsCaption={groupsCaption}
            defaultOption={me}
            defaultOptionLabel={t("Common:MeLabel")}
            employeeStatus={1}
            groupList={groups}
          />
        </FieldContainer>
        <FieldContainer
          className="members_container"
          isRequired={false}
          hasError={false}
          isVertical={true}
          labelText={t("Members")}
        >
          <ComboBox
            id="users-selector_button"
            tabIndex={3}
            options={[]}
            opened={isUsersSelectorOpen}
            isDisabled={inLoading}
            selectedOption={{
              key: 0,
              label: t("Translations:AddMembers"),
              default: true,
            }}
            scaled={true}
            size="content"
            toggleAction={onUsersSelectorClick}
            displayType="toggle"
          >
            <CatalogGuestIcon size="medium" />
          </ComboBox>
          <PeopleSelector
            isOpen={isUsersSelectorOpen}
            isMultiSelect={true}
            onSelect={onUsersSelectorSelect}
            onCancel={onCancelSelector}
            searchPlaceHolderLabel={t("SearchAddedMembers")}
            groupsCaption={groupsCaption}
            defaultOption={me}
            defaultOptionLabel={t("Common:MeLabel")}
            selectedOptions={newGroupMembers}
            employeeStatus={1}
            groupList={groups}
          />
        </FieldContainer>
        {newGroupMembers && newGroupMembers.length > 0 && (
          <>
            <div className="search_container">
              <SearchInput
                id="member-search"
                isDisabled={inLoading}
                scale={true}
                placeholder={t("SearchAddedMembers")}
                value={searchValue}
                onChange={onSearchChange}
              />
            </div>
            <div className="selected-members_container">
              {newGroupMembers.map((member) => (
                <SelectedItem
                  key={member.key}
                  text={member.label}
                  onClose={onSelectedItemClose.bind(this, member)}
                  isInline={false}
                  className="selected-item"
                  isDisabled={inLoading}
                />
              ))}
            </div>
          </>
        )}
        <div className="buttons_container">
          <Button
            label={buttonLabel}
            primary
            type="submit"
            isLoading={inLoading}
            size="big"
            tabIndex={4}
            onClick={onSave}
          />
          <Button
            label={t("Common:CancelButton")}
            className="cancel-button"
            size="big"
            isDisabled={inLoading}
            onClick={onCancel}
            tabIndex={5}
          />
        </div>
      </>
    </MainContainer>
  );
};

SectionBodyContent.propTypes = {
  group: PropTypes.object,
};

export default withRouter(
  inject(({ auth, peopleStore }) => {
    const { settingsStore, isLoaded, userStore } = auth;
    const { user: me } = userStore;
    const { customNames } = settingsStore;
    const { groupCaption, groupsCaption, groupHeadCaption } = customNames;

    const { filterStore, selectedGroupStore, groupsStore } = peopleStore;
    const { updateGroup, createGroup, groups: loadedGroups } = groupsStore;

    const groups = loadedGroups
      ? loadedGroups.map((g) => {
          return {
            key: g.id,
            label: g.name,
            total: 0,
          };
        })
      : [];

    const { filter, setFilterParams } = filterStore;
    const {
      targetedGroup,
      resetGroup,
      selectedGroup,
      setSelectedGroup,
      selectGroup,
    } = selectedGroupStore;

    const target = targetedGroup || {
      id: null,
      name: "",
      members: [],
      manager: null,
    };

    const { id, name, members, manager } = target;

    const groupMembers = members.map((m) => {
      return {
        key: m.id,
        label: m.displayName,
      };
    });

    const groupManager = manager
      ? {
          key: manager.id,
          label: manager.displayName,
        }
      : {
          key: GUID_EMPTY,
          label: "",
          default: true,
        };

    return {
      groupCaption,
      groupsCaption,
      groupHeadCaption,
      isLoaded,

      me,
      groups,
      id,
      name,
      groupManager,
      groupMembers,
      filter,
      setFilter: setFilterParams,
      selectGroup,
      updateGroup,
      createGroup,

      resetGroup: resetGroup,
      selectedGroup: selectedGroup,
      setSelectGroup: setSelectedGroup,
    };
  })(observer(withLoader(SectionBodyContent)(<Loaders.Group />)))
);
