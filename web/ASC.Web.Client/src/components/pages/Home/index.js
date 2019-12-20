import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from "react-router";
import { Loader, toastr, Text } from 'asc-web-components';
import { ModuleTile, PageLayout } from "asc-web-common";
import { useTranslation } from 'react-i18next';
import i18n from './i18n';
import styled from "styled-components";

const HomeContainer = styled.div`
    padding: 62px 15px 0 15px;
    margin: 0 auto;
    max-width: 1140px;
    width: 100%;
    box-sizing: border-box;
    /*justify-content: center;*/

    .home-modules {
        display: flex;
        flex-wrap: wrap;
        margin: 0 -15px;

        .home-module {
            flex-basis: 0;
            flex-grow: 1;
            max-width: 100%;
        }
    }

    .home-error-text {
        margin-top: 23px;
        padding: 0 30px;
        @media (min-width: 768px) {
            margin-left: 25%;
            flex: 0 0 50%;
            max-width: 50%;
        }
        @media (min-width: 576px) {
            flex: 0 0 100%;
            max-width: 100%;
        }
    }
`;

const Tiles = ({ modules, isPrimary, history }) => {
    let index = 0;

    return (
        <div className="home-modules">
            {
                modules.filter(m => m.isPrimary === isPrimary).map(module => (
                    <div className="home-module" key={++index}>
                        <ModuleTile {...module} onClick={() => window.open(module.link, '_self')} />
                    </div>
                ))
            }
        </div>
    );
};

Tiles.propTypes = {
    modules: PropTypes.array.isRequired,
    isPrimary: PropTypes.bool.isRequired,
    history: PropTypes.object.isRequired
};

const Body = ({ modules, match, history, isLoaded }) => {
    const { t } = useTranslation('translation', { i18n });
    const { params } = match;

    useEffect(() => {
        params.error && toastr.error(params.error);
    }, [params.error]);

    return (
        !isLoaded
            ? (
                <Loader className="pageLoader" type="rombs" size='40px' />
            )
            : (
                <HomeContainer>
                    <Tiles modules={modules} isPrimary={true} history={history} />
                    <Tiles modules={modules} isPrimary={false} history={history} />

                    {!modules || !modules.length ? (
                        <Text className="home-error-text" fontSize='14px' color="#c30">
                            {t('NoOneModulesAvailable')}
                        </Text> 
                    ) : null}
                </HomeContainer>
            )

    );
};

const Home = props => <PageLayout sectionBodyContent={<Body {...props} />} />;

Home.propTypes = {
    modules: PropTypes.array.isRequired,
    history: PropTypes.object.isRequired,
    isLoaded: PropTypes.bool
};

function mapStateToProps(state) {
    return {
        modules: state.auth.modules,
        isLoaded: state.auth.isLoaded
    };
}

export default connect(mapStateToProps)(withRouter(Home));