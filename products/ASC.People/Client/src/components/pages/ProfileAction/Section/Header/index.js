import React, { useCallback } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { IconButton } from "asc-web-components";
import { Headline } from "asc-web-common";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

const Wrapper = styled.div`
  display: grid;
  grid-template-columns: auto 1fr auto auto;
  align-items: center;

  .arrow-button {
    @media (max-width: 1024px) {
      padding: 8px 0 8px 8px;
      margin-left: -8px;
    }
  }

  .header-headline {
    margin-left: 16px;
  }
`;

const SectionHeaderContent = (props) => {
  const {
    profile,
    history,
    match,
    settings,
    filter,
    isEdit,
    setFilter,
    setIsVisibleDataLossDialog,
    toggleAvatarEditor,
    avatarEditorIsOpen,
  } = props;
  const { userCaption, guestCaption } = settings.customNames;
  const { type } = match.params;
  const { t } = useTranslation("ProfileAction");

  const headerText = avatarEditorIsOpen
    ? t("EditPhoto")
    : type
    ? type === "guest"
      ? t("CustomCreation", { user: guestCaption })
      : t("CustomCreation", { user: userCaption })
    : profile
    ? `${t("EditUserDialogTitle")} (${profile.displayName})`
    : "";

  const onClickBackHandler = () => {
    if (isEdit) {
      setIsVisibleDataLossDialog(true, onClickBack);
    } else {
      onClickBack();
    }
  };

  const setFilterAndReset = useCallback(
    (filter) => {
      props.resetProfile();
      setFilter(filter);
    },
    [props, setFilter]
  );

  const goBackAndReset = useCallback(() => {
    props.resetProfile();
    history.goBack();
  }, [history, props]);

  const onClickBack = useCallback(() => {
    avatarEditorIsOpen
      ? toggleAvatarEditor(false)
      : !profile || !document.referrer
      ? setFilterAndReset(filter)
      : goBackAndReset();
  }, [
    avatarEditorIsOpen,
    toggleAvatarEditor,
    profile,
    setFilterAndReset,
    filter,
    goBackAndReset,
  ]);
  return (
    <Wrapper>
      <IconButton
        iconName="ArrowPathIcon"
        color="#A3A9AE"
        size="17"
        hoverColor="#657077"
        isFill={true}
        onClick={onClickBackHandler}
        className="arrow-button"
      />
      <Headline className="header-headline" type="content" truncate={true}>
        {headerText}
      </Headline>
    </Wrapper>
  );
};

export default inject(({ auth, peopleStore }) => ({
  settings: auth.settingsStore,
  isEdit: peopleStore.editingFormStore.isEdit,
  setIsVisibleDataLossDialog:
    peopleStore.editingFormStore.setIsVisibleDataLossDialog,
  filter: peopleStore.filterStore.filter,
  setFilter: peopleStore.filterStore.setFilterParams,
  toggleAvatarEditor: peopleStore.avatarEditorStore.toggleAvatarEditor,
  resetProfile: peopleStore.targetUserStore.resetTargetUser,
  profile: peopleStore.targetUserStore.targetUser,
  avatarEditorIsOpen: peopleStore.avatarEditorStore.visible,
}))(observer(withRouter(SectionHeaderContent)));
