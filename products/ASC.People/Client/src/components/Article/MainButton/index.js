import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router';
import {
    MainButton,
    DropDownItem,
    toastr
} from "asc-web-components";
import { isAdmin } from '../../../store/auth/selectors';

class ArticleMainButtonContent extends React.Component {
    onDropDownItemClick = (link) => {
        this.props.history.push(link);
    };

    onNotImplementedClick = (text) => {
        toastr.success(text);
    };

    render() {
        console.log("People ArticleMainButtonContent render");
        const { isAdmin, settings } = this.props;
        return (
            isAdmin ?
                <MainButton
                    isDisabled={false}
                    isDropdown={true}
                    text={"Actions"}
                >
                    <DropDownItem
                        icon="CatalogEmployeeIcon"
                        label="New employee"
                        onClick={this.onDropDownItemClick.bind(this, `${settings.homepage}/create/user`)}
                    />
                    <DropDownItem
                        icon="CatalogGuestIcon"
                        label="New guest"
                        onClick={this.onDropDownItemClick.bind(this, `${settings.homepage}/create/guest`)}
                    />
                    <DropDownItem
                        icon="CatalogDepartmentsIcon"
                        label="New department"
                        onClick={this.onDropDownItemClick.bind(this, `${settings.homepage}/group/create`)}
                    />
                    <DropDownItem isSeparator />
                    <DropDownItem
                        icon="InvitationLinkIcon"
                        label="Invitation link"
                        onClick={this.onNotImplementedClick.bind(this, "Invitation link action")}
                    />
                    <DropDownItem
                        icon="PlaneIcon"
                        label="Invite again"
                        onClick={this.onNotImplementedClick.bind(this, "Invite again action")}
                    />
                    <DropDownItem
                        icon="ImportIcon"
                        label="Import people"
                        onClick={this.onNotImplementedClick.bind(this, "Import people action")}
                    />
                </MainButton>
                :
                <></>
        );
    };
};

ArticleMainButtonContent.propTypes = {
    isAdmin: PropTypes.bool.isRequired,
    history: PropTypes.object.isRequired
};

const mapStateToProps = (state) => {
    return {
        isAdmin: isAdmin(state.auth),
        settings: state.settings
    }
}

export default connect(mapStateToProps)(withRouter(ArticleMainButtonContent));