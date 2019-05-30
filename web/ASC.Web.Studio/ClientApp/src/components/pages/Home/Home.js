import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { getModules } from '../../../actions/getModulesActions';
import PropTypes from 'prop-types';
import { Container, Col, Row, Collapse } from 'reactstrap';
import { ModuleTile } from 'asc-web-components';

const PrimaryTile = ({ modules }) => (
    <Row>
        {
            modules.filter(m => m.isPrimary).map(module => (
                <Col key={0}>
                    <ModuleTile {...module} />
                </Col>
            ))
        }
    </Row>
);

const NotPrimaryTiles = ({ modules }) => {
    let index = 0;
    return (
        <Row>
            {modules.filter(m => !m.isPrimary).map(module => (
                <Col key={++index}>
                    <ModuleTile {...module} />
                </Col>
            ))
            }
        </Row>
    );
};

const Home = props => {
    const [modules, setModules] = useState([]);
    const [errorText, setErrorText] = useState("");
    const { getModules } = props;

    useEffect(() => {
        getModules()
            .then((res) => {
                console.log("getModules success", res);
                setModules(res.data.response);
            })
            .catch(e => {
                console.error("getModules error", e);
                setErrorText(e.message);
            });;
    }, [getModules]);

    return (
        <Container>
            <PrimaryTile modules={modules} />
            <NotPrimaryTiles modules={modules} />
            <Collapse isOpen={!!errorText}>
                <Row style={{ margin: "23px 0 0" }}>
                    <Col sm="12" md={{ size: 6, offset: 3 }}>
                        <div className="alert alert-danger">{errorText}</div>
                    </Col>
                </Row>
            </Collapse>
        </Container>
    );
};

Home.propTypes = {
    getModules: PropTypes.func.isRequired,
}

export default connect(null, { getModules })(Home);