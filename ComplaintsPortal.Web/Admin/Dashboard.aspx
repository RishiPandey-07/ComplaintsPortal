<%@ Page Title="My Dashboard" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="ComplaintsPortal.Web.Admin.Dashboard" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <!-- Chart.js local script included to work offline -->
    <script src="<%= ResolveUrl("~/Content/js/chart.min.js") %>"></script>

    <div class="page-header">
        <div>
            <h4 class="page-title"><i class="bi bi-speedometer2"></i> My Dashboard</h4>
            <p class="page-subtitle">Personalized overview of your requests and metrics.</p>
        </div>
    </div>

    <!-- Approver Panel -->
    <asp:Panel ID="pnlApprover" runat="server" Visible="false">
        <h5 class="fw-600 mb-3 text-secondary border-bottom pb-2">Approvals & Sanctions</h5>
        <div class="row mb-4 g-3">
            <div class="col-md-6 col-lg-3">
                <div class="portal-card h-100 text-center border-warning border-start border-4">
                    <div class="portal-card-body">
                        <h6 class="text-muted mb-2">Pending in My Queue</h6>
                        <h3 class="mb-0 text-warning" id="lblApproverPending" runat="server">0</h3>
                    </div>
                </div>
            </div>
            <div class="col-md-6 col-lg-3">
                <div class="portal-card h-100 text-center border-success border-start border-4">
                    <div class="portal-card-body">
                        <h6 class="text-muted mb-2">Sanctioned by Me</h6>
                        <h3 class="mb-0 text-success" id="lblApproverSanctioned" runat="server">0</h3>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Employee Panel -->
    <asp:Panel ID="pnlEmployee" runat="server">
        <h5 class="fw-600 mb-3 text-secondary border-bottom pb-2">My Requests</h5>
        <div class="row mb-4 g-3">
            <div class="col-md-6 col-lg-3">
                <div class="portal-card h-100 text-center border-primary border-start border-4">
                    <div class="portal-card-body">
                        <h6 class="text-muted mb-2">My Active Requests</h6>
                        <h3 class="mb-0 text-primary-portal" id="lblEmpActive" runat="server">0</h3>
                    </div>
                </div>
            </div>
            <div class="col-md-6 col-lg-3">
                <div class="portal-card h-100 text-center border-secondary border-start border-4">
                    <div class="portal-card-body">
                        <h6 class="text-muted mb-2">My Resolved Requests</h6>
                        <h3 class="mb-0 text-secondary" id="lblEmpResolved" runat="server">0</h3>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Global Admin Panel -->
    <asp:Panel ID="pnlGlobal" runat="server" Visible="false">
        <h5 class="fw-600 mb-3 text-secondary border-bottom pb-2">Global Overview</h5>
        <div class="row mb-4 g-3">
            <div class="col-md-4 col-lg-2">
                <div class="portal-card h-100 text-center">
                    <div class="portal-card-body">
                        <h6 class="text-muted mb-2">Total Requests</h6>
                        <h3 class="mb-0 text-primary-portal" id="lblTotal" runat="server">0</h3>
                    </div>
                </div>
            </div>
            <div class="col-md-4 col-lg-2">
                <div class="portal-card h-100 text-center">
                    <div class="portal-card-body">
                        <h6 class="text-muted mb-2">Pending</h6>
                        <h3 class="mb-0 text-warning" id="lblPending" runat="server">0</h3>
                    </div>
                </div>
            </div>
            <div class="col-md-4 col-lg-2">
                <div class="portal-card h-100 text-center">
                    <div class="portal-card-body">
                        <h6 class="text-muted mb-2">In Progress</h6>
                        <h3 class="mb-0 text-info" id="lblInProgress" runat="server">0</h3>
                    </div>
                </div>
            </div>
            <div class="col-md-4 col-lg-2">
                <div class="portal-card h-100 text-center">
                    <div class="portal-card-body">
                        <h6 class="text-muted mb-2">Completed</h6>
                        <h3 class="mb-0 text-success" id="lblCompleted" runat="server">0</h3>
                    </div>
                </div>
            </div>
            <div class="col-md-4 col-lg-2">
                <div class="portal-card h-100 text-center">
                    <div class="portal-card-body">
                        <h6 class="text-muted mb-2">Rejected</h6>
                        <h3 class="mb-0 text-secondary" id="lblRejected" runat="server">0</h3>
                    </div>
                </div>
            </div>
            <div class="col-md-4 col-lg-2">
                <div class="portal-card h-100 text-center">
                    <div class="portal-card-body">
                        <h6 class="text-muted mb-2">SLA Breached</h6>
                        <h3 class="mb-0 text-danger" id="lblSlaBreached" runat="server">0</h3>
                    </div>
                </div>
            </div>
        </div>

        <!-- Charts Row -->
        <div class="row">
            <div class="col-md-6 mb-4">
                <div class="portal-card h-100">
                    <div class="portal-card-header">
                        <h5 class="portal-card-title">Requests by Status</h5>
                    </div>
                    <div class="portal-card-body">
                        <canvas id="statusChart" style="max-height: 300px;"></canvas>
                    </div>
                </div>
            </div>
            <div class="col-md-6 mb-4">
                <div class="portal-card h-100">
                    <div class="portal-card-header">
                        <h5 class="portal-card-title">Top Request Types</h5>
                    </div>
                    <div class="portal-card-body">
                        <canvas id="typeChart" style="max-height: 300px;"></canvas>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            // Chart.js global defaults for premium look
            if(typeof Chart !== 'undefined') {
                Chart.defaults.font.family = "'Roboto', 'Helvetica Neue', 'Helvetica', 'Arial', sans-serif";
                Chart.defaults.color = '#495057';

                // Only render charts if the containers exist (Admin view)
                if (document.getElementById('statusChart')) {
                    const statusLabels = JSON.parse('<%= StatusLabelsJson %>');
                    const statusValues = JSON.parse('<%= StatusValuesJson %>');

                    const typeLabels = JSON.parse('<%= TypeLabelsJson %>');
                    const typeValues = JSON.parse('<%= TypeValuesJson %>');

                    // Status Doughnut Chart
                    const ctxStatus = document.getElementById('statusChart').getContext('2d');
                    new Chart(ctxStatus, {
                        type: 'doughnut',
                        data: {
                            labels: statusLabels,
                            datasets: [{
                                data: statusValues,
                                backgroundColor: [
                                    '#0d6efd', '#ffc107', '#17a2b8', '#198754', '#6c757d', '#dc3545'
                                ],
                                borderWidth: 0,
                                hoverOffset: 4
                            }]
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: false,
                            plugins: {
                                legend: { position: 'right' }
                            },
                            cutout: '70%'
                        }
                    });

                    // Type Bar Chart
                    const ctxType = document.getElementById('typeChart').getContext('2d');
                    new Chart(ctxType, {
                        type: 'bar',
                        data: {
                            labels: typeLabels,
                            datasets: [{
                                label: 'Requests',
                                data: typeValues,
                                backgroundColor: '#4361ee',
                                borderRadius: 4
                            }]
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: false,
                            plugins: {
                                legend: { display: false }
                            },
                            scales: {
                                y: { beginAtZero: true, grid: { color: '#f1f3f5' } },
                                x: { grid: { display: false } }
                            }
                        }
                    });
                }
            }
        });
    </script>
</asp:Content>
