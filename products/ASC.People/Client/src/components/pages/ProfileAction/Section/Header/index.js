import React, { useCallback } from "react";
import styled from "styled-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { IconButton } from "asc-web-components";
import { Headline } from "asc-web-common";
import { useTranslation } from "react-i18next";
import {
  setFilter,
  setIsVisibleDataLossDialog,
} from "../../../../../store/people/actions";

const Wrapper = styled.div`
  display: flex;
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
    editingForm,
    setFilter,
    setIsVisibleDataLossDialog,
  } = props;
  const { userCaption, guestCaption } = settings.customNames;
  const { type } = match.params;
  const { t } = useTranslation();

  const headerText = type
    ? type === "guest"
      ? t("CustomCreation", { user: guestCaption })
      : t("CustomCreation", { user: userCaption })
    : profile
    ? `${t("EditUserDialogTitle")} (${profile.displayName})`
    : "";

  const onClickBackHandler = () => {
    if (editingForm.isEdit) {
      setIsVisibleDataLossDialog(true, onClickBack);
    } else {
      onClickBack();
    }
  };
  const onClickBack = useCallback(() => {
    !profile || !document.referrer ? setFilter(filter) : history.goBack();
  }, [history, profile, setFilter, filter]);

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

function mapStateToProps(state) {
  return {
    profile: state.profile.targetUser,
    settings: state.auth.settings,
    filter: state.people.filter,
    editingForm: state.people.editingForm,
  };
}

export default connect(mapStateToProps, {
  setFilter,
  setIsVisibleDataLossDialog,
})(withRouter(SectionHeaderContent));
