import React, { useCallback } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import IconButton from "@docspace/components/icon-button";
import Headline from "@docspace/common/components/Headline";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { isMobileOnly } from "react-device-detect";

import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";

import Loaders from "@docspace/common/components/Loaders";
import withPeopleLoader from "../../../../HOCs/withPeopleLoader";

const homepage = config.homepage;

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
    customNames,
    filter,
    isEdit,
    setFilter,
    setIsVisibleDataLossDialog,
    toggleAvatarEditor,
    avatarEditorIsOpen,
    isMy,
    isEditTargetUser,
  } = props;
  const { userCaption, guestCaption } = customNames;
  const { type } = match.params;
  const { t } = useTranslation("ProfileAction");

  const headerText = avatarEditorIsOpen
    ? t("EditPhoto")
    : type
    ? type === "guest"
      ? t("CustomCreation", { user: guestCaption })
      : t("CustomCreation", { user: userCaption })
    : profile
    ? isMobileOnly
      ? t("Common:Editing")
      : t("EditUserDialogTitle")
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
    if (isMy) {
      return history.goBack();
    }

    if (!isEditTargetUser && (!profile || !document.referrer)) {
      setFilterAndReset(filter);
      const urlFilter = filter.toUrlParams();
      return history.push(
        combineUrl(
          AppServerConfig.proxyURL,
          homepage,
          `/accounts/filter?${urlFilter}`
        )
      );
    }

    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        homepage,
        `/accounts/view/${profile.userName}`
      )
    );

    return props.resetProfile();
  }, [history, props]);

  const onClickBack = useCallback(() => {
    avatarEditorIsOpen ? toggleAvatarEditor(false) : goBackAndReset();
  }, [avatarEditorIsOpen, toggleAvatarEditor, profile, filter, goBackAndReset]);
  return (
    <Wrapper>
      <IconButton
        iconName="/static/images/arrow.path.react.svg"
        size="17"
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

export default withRouter(
  inject(({ auth, peopleStore }) => ({
    customNames: auth.settingsStore.customNames,
    isEdit: peopleStore.editingFormStore.isEdit,
    setIsVisibleDataLossDialog:
      peopleStore.editingFormStore.setIsVisibleDataLossDialog,
    filter: peopleStore.filterStore.filter,
    setFilter: peopleStore.filterStore.setFilterParams,
    toggleAvatarEditor: peopleStore.avatarEditorStore.toggleAvatarEditor,
    resetProfile: peopleStore.targetUserStore.resetTargetUser,
    profile: peopleStore.targetUserStore.targetUser,
    avatarEditorIsOpen: peopleStore.avatarEditorStore.visible,
    isEditTargetUser: peopleStore.targetUserStore.isEditTargetUser,
  }))(
    withPeopleLoader(observer(SectionHeaderContent))(<Loaders.SectionHeader />)
  )
);
