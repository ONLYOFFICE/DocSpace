import React from "react";
import { withRouter } from "react-router";
import PropTypes from "prop-types";

import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import history from "@appserver/common/history";

import { withTranslation, Trans } from "react-i18next";
import api from "@appserver/common/api";
import toastr from "studio/toastr";
import ModalDialogContainer from "../ModalDialogContainer";
import { inject, observer } from "mobx-react";
import config from "../../../../package.json";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";

const { deleteUser } = api.people; //TODO: Move to action
const { Filter } = api;

class DeleteProfileEverDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isRequestRunning: false,
    };
  }
  onDeleteProfileEver = () => {
    const { onClose, filter, fetchPeople, user, t } = this.props;
    this.setState({ isRequestRunning: true }, () => {
      deleteUser(user.id)
        .then((res) => {
          toastr.success(t("SuccessfullyDeleteUserInfoMessage"));
          return fetchPeople(filter);
        })
        .catch((error) => toastr.error(error))
        .finally(onClose);
    });
  };

  onReassignDataClick = () => {
    const { homepage, user } = this.props;
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        homepage,
        `/reassign/${user.userName}`
      )
    );
  };

  render() {
    console.log("DeleteProfileEverDialog render");
    const { t, tReady, visible, user, onClose, userCaption } = this.props;
    const { isRequestRunning } = this.state;

    return (
      <ModalDialogContainer
        isLoading={!tReady}
        visible={visible}
        onClose={onClose}
      >
        <ModalDialog.Header>{t("Confirmation")}</ModalDialog.Header>
        <ModalDialog.Body>
          <Text>
            <Trans
              i18nKey="DeleteUserConfirmation"
              ns="DeleteProfileEverDialog"
              t={t}
            >
              {{ userCaption }} <strong>{{ user: user.displayName }}</strong>{" "}
              will be deleted.
            </Trans>
          </Text>
          <Text>{t("Translations:NotBeUndone")}</Text>
          {/* <Text color="#c30" fontSize="18px" className="warning-text">
              {t("Common:Warning")}!
            </Text>
            <Text>{t("DeleteUserDataConfirmation")}</Text> */}
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            key="OKBtn"
            label={t("Common:OKButton")}
            size="medium"
            primary={true}
            onClick={this.onDeleteProfileEver}
            isLoading={isRequestRunning}
          />
          {/* <Button
              className="button-dialog"
              key="ReassignBtn"
              label={t("Translations:ReassignData")}
              size="medium"
              onClick={this.onReassignDataClick}
              isDisabled={isRequestRunning}
            /> */}
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

const DeleteProfileEverDialog = withTranslation([
  "DeleteProfileEverDialog",
  "Common",
  "Translations",
])(DeleteProfileEverDialogComponent);

DeleteProfileEverDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  user: PropTypes.object.isRequired,
  filter: PropTypes.instanceOf(Filter).isRequired,
  fetchPeople: PropTypes.func.isRequired,
};

export default withRouter(
  inject(({ auth, peopleStore }) => ({
    homepage: config.homepage,
    userCaption: auth.settingsStore.customNames.userCaption,
    fetchPeople: peopleStore.usersStore.getUsersList,
    filter: peopleStore.filterStore.filter,
  }))(observer(DeleteProfileEverDialog))
);
