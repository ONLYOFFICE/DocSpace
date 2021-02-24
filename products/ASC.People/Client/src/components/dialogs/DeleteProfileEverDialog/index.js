import React from "react";
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import { ModalDialog, Button, Text } from "asc-web-components";
import { withTranslation, Trans } from "react-i18next";
import { api, toastr } from "asc-web-common";

import ModalDialogContainer from "../ModalDialogContainer";
import { inject, observer } from "mobx-react";

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
    const { history, settings, user } = this.props;
    history.push(`${settings.homepage}/reassign/${user.userName}`);
  };

  render() {
    console.log("DeleteProfileEverDialog render");
    const { t, visible, user, onClose, userCaption } = this.props;
    const { isRequestRunning } = this.state;

    return (
      <ModalDialogContainer>
        <ModalDialog visible={visible} onClose={onClose}>
          <ModalDialog.Header>{t("Confirmation")}</ModalDialog.Header>
          <ModalDialog.Body>
            <Text>
              <Trans
                i18nKey="DeleteUserConfirmation"
                ns="DeleteProfileEverDialog"
              >
                {{ userCaption }} <strong>{{ user: user.displayName }}</strong>{" "}
                will be deleted.
              </Trans>
            </Text>
            <Text>{t("NotBeUndone")}</Text>
            <Text color="#c30" fontSize="18px" className="warning-text">
              {t("Warning")}
            </Text>
            <Text>{t("DeleteUserDataConfirmation")}</Text>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              key="OKBtn"
              label={t("OKButton")}
              size="medium"
              primary={true}
              onClick={this.onDeleteProfileEver}
              isLoading={isRequestRunning}
            />
            <Button
              className="button-dialog"
              key="ReassignBtn"
              label={t("ReassignData")}
              size="medium"
              onClick={this.onReassignDataClick}
              isDisabled={isRequestRunning}
            />
          </ModalDialog.Footer>
        </ModalDialog>
      </ModalDialogContainer>
    );
  }
}

const DeleteProfileEverDialog = withTranslation("DeleteProfileEverDialog")(
  DeleteProfileEverDialogComponent
);

DeleteProfileEverDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  user: PropTypes.object.isRequired,
  filter: PropTypes.instanceOf(Filter).isRequired,
  fetchPeople: PropTypes.func.isRequired,
  settings: PropTypes.object.isRequired,
  history: PropTypes.object.isRequired,
};

export default inject(({ auth, peopleStore }) => ({
  userCaption: auth.settingsStore.customNames.userCaption,
  fetchPeople: peopleStore.usersStore.getUsersList,
}))(observer(withRouter(DeleteProfileEverDialog)));
