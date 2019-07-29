import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router';
import config from '../../../../package.json';
import {
    MainButton,
    DropDownItem,
} from "asc-web-components";


const ArticleMainButtonContent = ({ isAdmin, history }) => {
    console.log("People ArticleMainButtonContent");
    return (
        isAdmin ?
            <MainButton
                isDisabled={false}
                isDropdown={true}
                text={"Actions"}
            >
                <DropDownItem
                    label="New employee"
                    onClick={() => history.push(`${config.homepage}/create/user`)}
                />
                <DropDownItem
                    label="New guest"
                    onClick={() => history.push(`${config.homepage}/create/guest`)}
                />
                <DropDownItem
                    label="New department"
                    onClick={() => console.log("New department clicked")}
                />
                <DropDownItem isSeparator />
                <DropDownItem
                    label="Invitation link"
                    onClick={() => console.log("Invitation link clicked")}
                />
                <DropDownItem
                    label="Invite again"
                    onClick={() => console.log("Invite again clicked")}
                />
                <DropDownItem
                    label="Import people"
                    onClick={() => console.log("Import people clicked")}
                />
            </MainButton>
            :
            <></>
    )
};

ArticleMainButtonContent.propTypes = {
    isAdmin: PropTypes.bool.isRequired,
    history: PropTypes.object.isRequired
};

const mapStateToProps = (state) => {
    return {
        isAdmin: state.auth.user.isAdmin || state.auth.user.isOwner
    }
}

export default connect(mapStateToProps)(withRouter(ArticleMainButtonContent));