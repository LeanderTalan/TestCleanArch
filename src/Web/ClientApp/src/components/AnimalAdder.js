import React, { Component } from 'react';

export class AnimalAdder extends Component {
  static displayName = AnimalAdder.name;

  constructor(props) {
    super(props);
    
    const today = new Date().toISOString().split('T')[0];
    
    this.state = {
      cowName: '',
      cowBirthDate: today,
      chickenName: '',
      chickenBirthDate: today,
      message: ''
    };

    this.handleCowNameChange = this.handleCowNameChange.bind(this);
    this.handleCowBirthDateChange = this.handleCowBirthDateChange.bind(this);
    this.handleChickenNameChange = this.handleChickenNameChange.bind(this);
    this.handleChickenBirthDateChange = this.handleChickenBirthDateChange.bind(this);
    this.addCow = this.addCow.bind(this);
    this.addChicken = this.addChicken.bind(this);
  }

  handleCowNameChange(e) {
    this.setState({ cowName: e.target.value });
  }

  handleCowBirthDateChange(e) {
    this.setState({ cowBirthDate: e.target.value });
  }

  handleChickenNameChange(e) {
    this.setState({ chickenName: e.target.value });
  }

  handleChickenBirthDateChange(e) {
    this.setState({ chickenBirthDate: e.target.value });
  }

  async addCow() {
    const name = (this.state.cowName || '').trim();
    const birthDate = (this.state.cowBirthDate || '').trim();
    if (!name || !birthDate) {
      this.setState({ message: 'Enter a cow name and birth date' });
      return;
    }

    const res = await fetch('/api/Cows', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ cowName: name, birthDate: birthDate })
    });

    if (res.ok) {
      const today = new Date().toISOString().split('T')[0];
      this.setState({ cowName: '', cowBirthDate: today, message: 'Cow added' });
    } else {
      this.setState({ message: 'Failed to add cow' });
    }

    setTimeout(() => this.setState({ message: '' }), 2500);
  }

  async addChicken() {
    const name = (this.state.chickenName || '').trim();
    const birthDate = (this.state.chickenBirthDate || '').trim();
    if (!name || !birthDate) {
      this.setState({ message: 'Enter a chicken name and birth date' });
      return;
    }

    const res = await fetch('/api/Chickens', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ chickenName: name, birthDate: birthDate })
    });

    if (res.ok) {
      const today = new Date().toISOString().split('T')[0];
      this.setState({ chickenName: '', chickenBirthDate: today, message: 'Chicken added' });
    } else {
      this.setState({ message: 'Failed to add chicken' });
    }

    setTimeout(() => this.setState({ message: '' }), 2500);
  }

  render() {
    const { cowName, cowBirthDate, chickenName, chickenBirthDate, message } = this.state;

    return (
      <div>
        <h1>Animal Adder</h1>

        {message && <div className="alert alert-info">{message}</div>}

        <div className="card mb-3">
          <div className="card-body">
            <h5 className="card-title">Add a Cow</h5>
            <div className="mb-2">
              <input
                type="text"
                className="form-control"
                placeholder="Cow name"
                value={cowName}
                onChange={this.handleCowNameChange}
              />
            </div>
            <div className="input-group">
              <input
                type="date"
                className="form-control"
                placeholder="Birth date"
                value={cowBirthDate}
                onChange={this.handleCowBirthDateChange}
              />
              <button className="btn btn-primary" onClick={this.addCow}>
                Add Cow
              </button>
            </div>
          </div>
        </div>

        <div className="card mb-3">
          <div className="card-body">
            <h5 className="card-title">Add a Chicken</h5>
            <div className="mb-2">
              <input
                type="text"
                className="form-control"
                placeholder="Chicken name"
                value={chickenName}
                onChange={this.handleChickenNameChange}
              />
            </div>
            <div className="input-group">
              <input
                type="date"
                className="form-control"
                placeholder="Birth date"
                value={chickenBirthDate}
                onChange={this.handleChickenBirthDateChange}
              />
              <button className="btn btn-primary" onClick={this.addChicken}>
                Add Chicken
              </button>
            </div>
          </div>
        </div>
      </div>
    );
  }
}