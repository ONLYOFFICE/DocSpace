import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router';
import {
    MainButton,
    DropDownItem,
    toastr
} from "asc-web-components";
import InviteDialog from './../../dialogs/Invite';
import { isAdmin } from '../../../store/auth/selectors';
import { withTranslation, I18nextProvider } from 'react-i18next';
import i18n from '../i18n';
import { typeUser, typeGuest, department } from './../../../helpers/customNames';

class PureArticleMainButtonContent extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            dialogVisible: false,
        }
    }

    onDropDownItemClick = (link) => {
        this.props.history.push(link);
    };

    onNotImplementedClick = (text) => {
        toastr.success(text);
    };

    toggleDialogVisible = () => {
        this.setState({ dialogVisible: !this.state.dialogVisible }, function () {
            this.state.dialogVisible && this.clickChild();
        });
    }

    onSetInviteDialogClick = click => this.clickChild = click;
    render() {
        console.log("People ArticleMainButtonContent render");
        const { isAdmin, settings, t } = this.props;
        return (
            isAdmin ?
                <>
                    <MainButton
                        isDisabled={false}
                        isDropdown={true}
                        text={t('Actions')}
                    >
                        <DropDownItem
                            icon="CatalogEmployeeIcon"
                            label={t('CustomNewEmployee', { typeUser })}
                            onClick={this.onDropDownItemClick.bind(this, `${settings.homepage}/create/user`)}
                        />
                        <DropDownItem
                            icon="CatalogGuestIcon"
                            label={t('CustomNewGuest', { typeGuest })}
                            onClick={this.onDropDownItemClick.bind(this, `${settings.homepage}/create/guest`)}
                        />
                        <DropDownItem
                            icon="CatalogDepartmentsIcon"
                            label={t('CustomNewDepartment', { department })}
                            onClick={this.onDropDownItemClick.bind(this, `${settings.homepage}/group/create`)}
                        />
                        <DropDownItem isSeparator />
                        <DropDownItem
                            icon="InvitationLinkIcon"
                            label={t('InviteLinkTitle')}
                            onClick={this.toggleDialogVisible}
                        />
                        <DropDownItem
                            icon="PlaneIcon"
                            label={t('LblInviteAgain')}
                            onClick={this.onNotImplementedClick.bind(this, "Invite again action")}
                        />
                        <DropDownItem
                            icon="ImportIcon"
                            label={t('ImportPeople')}
                            onClick={this.onNotImplementedClick.bind(this, "Import people action")}
                        />
                    </MainButton>
                    <InviteDialog
                        setClick={this.onSetInviteDialogClick}
                        visible={this.state.dialogVisible}
                        onClose={this.toggleDialogVisible}
                        onCloseButton={this.toggleDialogVisible}
                    />
                </>
                :
                <></>
        );
    };
};


const mapStateToProps = (state) => {
    return {
        isAdmin: isAdmin(state.auth.user),
        settings: state.auth.settings
    }
}

const ArticleMainButtonContentContainer = withTranslation()(PureArticleMainButtonContent);

const ArticleMainButtonContent = (props) => <I18nextProvider i18n={i18n}><ArticleMainButtonContentContainer {...props} /></I18nextProvider>;

ArticleMainButtonContent.propTypes = {
    isAdmin: PropTypes.bool.isRequired,
    history: PropTypes.object.isRequired
};

export default connect(mapStateToProps)(withRouter(ArticleMainButtonContent));